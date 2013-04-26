include FileUtils

namespace :ripple do
	desc "Restores nuget package files"
	task :restore do
	  puts 'Restoring all the nuget package files'
      sh 'ripple restore'
	end

	desc "Updates nuget package files to the latest"
	task :update do
	  puts 'Updating all the nuget package files'
	  sh 'ripple update'
	end

	desc "creates a history file for nuget dependencies"
	task :history do
	  sh 'ripple history'
	end
	
	desc "publishes all the nuget's published by this solution"
	task :publish do
	  nuget_api_key = ENV['apikey']
	  server = ENV['server']
	  cmd = "ripple publish #{BUILD_NUMBER} #{nuget_api_key}"
	  cmd = cmd + " --server #{server}" unless server.nil?
	  sh cmd
	end
	
	desc "packages the nuget files from the nuspec files in packaging/nuget and publishes to /artifacts"
	task :package => [:history] do
		COMPILE_TARGET = 'release'
		Rake::Task["compile"].execute
	
		sh "ripple local-nuget --version #{BUILD_NUMBER} --destination artifacts"
	end
end

class NUnitRunner
	include FileTest

	def initialize(paths)
		@sourceDir = paths.fetch(:source, 'source')
		@resultsDir = paths.fetch(:results, 'results')
		@compilePlatform = paths.fetch(:platform, '')
		@compileTarget = paths.fetch(:compilemode, 'debug')
	
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

namespace :docs do
	desc "Tries to run a documentation project hosted in FubuWorld"
	task :run do
		sh "fubudocs run -o"
	end
	
	desc "Tries to run the documentation projects in this solution in a 'watched' mode in Firefox"
	task :run_firefox do
		sh "fubudocs run --watched --browser Firefox"
	end
	
	desc "Tries to run the documentation projects in this solution in a 'watched' mode in Firefox"
	task :run_chrome do
		sh "fubudocs run --watched --browser Chrome"
	end

	desc "'Bottles' up a single project in the solution with 'Docs' in its name"
	task :bottle do
		sh "fubudocs bottle"
	end

	desc "Gathers up code snippets from the solution into the Docs project"
	task :snippets do
		sh "fubudocs snippets"
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

def waitfor(&block)
  checks = 0
  until block.call || checks >10 
    sleep 0.5
    checks += 1
  end
  raise 'waitfor timeout expired' if checks > 10
end

def cleanDirectory(dir)
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