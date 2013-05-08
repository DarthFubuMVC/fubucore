include FileUtils
include FileTest



class NUnitRunner
	include FileTest

	def initialize(paths)
		@sourceDir = paths.fetch(:source, 'src')
		@resultsDir = paths.fetch(:results, 'results')
		@compilePlatform = paths.fetch(:platform, 'x86')
		@compileTarget = paths.fetch(:compilemode, 'debug')
	puts "COMPILE TARGET IS #{@compilePlatform}"
		@nunitExe = Nuget.tool("NUnit", "nunit-console#{(@compilePlatform.empty? ? '' : "-#{@compilePlatform}")}.exe") + Platform.switch("nothread")
	end
	
	def executeTests(assemblies)
		Dir.mkdir @resultsDir unless exists?(@resultsDir)
		
		assemblies.each do |assem|
			file = File.expand_path("#{@sourceDir}/#{assem}/bin/#{@compileTarget}/#{assem}.dll")
			sh Platform.runtime("#{@nunitExe} -xml=#{@resultsDir}/#{assem}-TestResults.xml \"#{file}\"")
		end
	end
	
	def executeTestsInFile(file)
	  if !File.exist?(file)
		throw "File #{file} does not exist"
	  end
	  
	  tests = Array.new

	  file = File.new(file, "r")
	  assemblies = file.readlines()
	  assemblies.each do |a|
		test = a.gsub("\r\n", "").gsub("\n", "")
		tests.push(test)
	  end
	  file.close
	  
	  if (!tests.empty?)
	    executeTests tests
	  end
	end
end



module Platform

  def self.is_nix
    !RUBY_PLATFORM.match("linux|darwin").nil?
  end

  def self.runtime(cmd)
    command = cmd
    if self.is_nix
      runtime = (CLR_TOOLS_VERSION || "v4.0.30319")
      command = "mono --runtime=#{runtime} #{cmd}"
    end
    command
  end

  def self.switch(arg)
    sw = self.is_nix ? " -" : " /"
    sw + arg
  end

end



class MSBuildRunner
	def self.compile(attributes)
		compileTarget = attributes.fetch(:compilemode, 'debug')
	    solutionFile = attributes[:solutionfile]
	    
	    attributes[:projFile] = solutionFile
	    attributes[:properties] ||= []
	    attributes[:properties] << "Configuration=#{compileTarget}"
	    attributes[:extraSwitches] = ["v:m", "t:rebuild"]
		  attributes[:extraSwitches] << "maxcpucount:2" unless Platform.is_nix

      self.runProjFile(attributes);
	end
	
	def self.runProjFile(attributes)
		version = attributes.fetch(:clrversion, 'v4.0.30319')
		compileTarget = attributes.fetch(:compilemode, 'debug')
	    projFile = attributes[:projFile]
		
    if Platform.is_nix
      msbuildFile = `which xbuild`.chop
      attributes[:properties] << "TargetFrameworkProfile="""""
    else
		  frameworkDir = File.join(ENV['windir'].dup, 'Microsoft.NET', 'Framework', version)
		  msbuildFile = File.join(frameworkDir, 'msbuild.exe')
   end

    properties = attributes.fetch(:properties, [])
		
		switchesValue = ""
		
		properties.each do |prop|
			switchesValue += " /property:#{prop}"
		end	
		
		extraSwitches = attributes.fetch(:extraSwitches, [])	
		
		extraSwitches.each do |switch|
			switchesValue += " /#{switch}"
		end	
		
		targets = attributes.fetch(:targets, [])
		targetsValue = ""
		targets.each do |target|
			targetsValue += " /t:#{target}"
		end
		
		sh "#{msbuildFile} #{projFile} #{targetsValue} #{switchesValue}"
	end
end

def copyOutputFiles(fromDir, filePattern, outDir)
  Dir.glob(File.join(fromDir, filePattern)){|file| 		
	copy(file, outDir) if File.file?(file)
  } 
end

def cleanTask(array)
	desc "Prepares the working directory for a new build"
	task :clean do
	  array.each do |a|
		cleanDirectory a
	  end
	end
end




def waitfor(&block)
  checks = 0
  until block.call || checks >10 
    sleep 0.5
    checks += 1
  end
  raise 'waitfor timeout expired' if checks > 10
end

def cleanDirectory(dir)
  puts 'Cleaning directory ' + dir
  FileUtils.rm_rf dir;
  waitfor { !exists?(dir) }
  Dir.mkdir dir
end

def cleanFile(file)
  File.delete file unless !File.exist?(file)
end

module Nuget
  def self.tool(package, tool)
    nugetDir = Dir.glob(File.join(package_root,"#{package}*")).sort.last
    return File.join(nugetDir, "tools", tool) if File.directory?(nugetDir)
        
    
    File.join(Dir.glob(File.join(package_root,"#{package}.[0-9]*")).sort.last, "tools", tool)
  end
  
  def self.package_root
    root = nil
    
    packroots = Dir.glob("{source,src}/packages")

    return packroots.last if packroots.length > 0

    Dir.glob("{source,src}").each do |d|
      packroot = File.join d, "packages"
      FileUtils.mkdir_p(packroot) 
      root = packroot
    end       

    root
  end
end


module FubuRake
  class SolutionTasks
    @clean = []
	@compile = nil
	@assembly_info = nil
	@ripple_enabled = false
	@fubudocs_enabled = false
	@options = nil
	
	attr_accessor :clean, :compile, :assembly_info, :ripple_enabled, :fubudocs_enabled, :options
	
	
  end
  
  class Solution
    def initialize(&block)
	  tasks = SolutionTasks.new
	  block.call(tasks)
	  
	  options = tasks.options
	  options ||= {}
	  
	  options = {
		:compilemode => ENV['config'].nil? ? "Debug" : ENV['config'],
		:clrversion => 'v4.0.30319',
		:platform => 'x86',
		:unit_test_list_file => 'TESTS.txt',
		:unit_test_projects => [],
		:source => 'src'}.merge(options)

	  tasks.clean ||= []
	  
	  if tasks.ripple_enabled
	    require File.join(File.dirname(__FILE__), 'ripple')
		
		tasks.clean << 'artifacts'
		
		#TODO -- add more stuff in to tasks
	  end
	  
	  if tasks.fubudocs_enabled
		require File.join(File.dirname(__FILE__), 'fubudocs')
	  end
	  
	  if tasks.assembly_info != nil
	    versionTask = Rake::Task.define_task :version do
			tc_build_number = ENV["BUILD_NUMBER"]
			build_revision = tc_build_number || Time.new.strftime('5%H%M')
		    asm_version = BUILD_VERSION + ".0"
			build_number = "#{BUILD_VERSION}.#{build_revision}"
		  
		    begin
			  commit = `git log -1 --pretty=format:%H`
		    rescue
			  commit = "git unavailable"
		    end
		    puts "##teamcity[buildNumber '#{build_number}']" unless tc_build_number.nil?
		    puts "Version: #{build_number}" if tc_build_number.nil?
			
			options = {
				:trademark => commit, 
				:product_name => 'CHANGEME', 
				:description => build_number, 
				:version => asm_version, 
				:file_version => build_number,
				:informational_version => asm_version,
				:copyright => 'CHANGEME',
				:output_file => 'src/CommonAssemblyInfo.cs'
			}
			
			options = options.merge(tasks.assembly_info)
			
			File.open(options[:output_file], 'w') do |file|
				file.write "using System.Reflection;\n"
				file.write "using System.Runtime.InteropServices;\n"
				file.write "[assembly: AssemblyDescription(\"#{options[:description]}\")]\n"
				file.write "[assembly: AssemblyProduct(\"#{options[:product_name]}\")]\n"
				file.write "[assembly: AssemblyCopyright(\"#{options[:copyright]}\")]\n"
				file.write "[assembly: AssemblyTrademark(\"#{options[:trademark]}\")]\n"
				file.write "[assembly: AssemblyVersion(\"#{options[:version]}\")]\n"
				file.write "[assembly: AssemblyFileVersion(\"#{options[:file_version]}\")]\n"
				file.write "[assembly: AssemblyInformationalVersion(\"#{options[:informational_version]}\")]\n"
			end
		end
		
		versionTask.add_description "Update the version information for the build"
	  end
	  
	  if tasks.clean.any?
	    cleanTask = Rake::Task.define_task :clean do
		  tasks.clean.each do |dir|
			cleanDirectory dir
		  end
		end
	  
		cleanTask.add_description "Prepares the working directory for a new build"
	  end

	  if tasks.compile != nil
		compileTask = Rake::Task.define_task :compile do
		  MSBuildRunner.compile options.merge(tasks.compile)
		end
		
		compileTask.add_description "Compiles the application"
		
		if (tasks.clean.any?)
			compileTask.enhance [:clean]
		end

		if (tasks.assembly_info != nil)
			compileTask.enhance [:version]
		end
		
		if (tasks.ripple_enabled)
			compileTask.enhance ["ripple:restore"]
		end 
	  end
	  
	  nunitTask = nil
	  if options[:unit_test_projects].any?
		nunitTask = Rake::Task.define_task :unit_test do
		  runner = NUnitRunner.new options
		  runner.executeTests options[:unit_test_projects]
		end
	  elsif options[:unit_test_list_file] != nil
		file = options[:unit_test_list_file]
	  
		nunitTask = Rake::Task.define_task :unit_test do
		  runner = NUnitRunner.new options
		  runner.executeTestsInFile file
		end
	  end
	  
	  if nunitTask != nil
		nunitTask.enhance [:compile]
		nunitTask.add_description "Runs unit tests"
	  end
	end
  end
end