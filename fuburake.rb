include FileUtils
include FileTest

require File.join(File.dirname(__FILE__), 'nunit')
require File.join(File.dirname(__FILE__), 'msbuild')
require File.join(File.dirname(__FILE__), 'nuget')
require File.join(File.dirname(__FILE__), 'platform')

module FubuRake
  class SolutionTasks
	attr_accessor :clean, 
		:compile, 
		:assembly_info,
		:ripple_enabled, 
		:fubudocs_enabled, 
		:options, 
		:defaults,
		:ci_steps
  end
  
  class Solution
    def initialize(&block)
	  tasks = SolutionTasks.new
	  block.call(tasks)
	  
	  make_default_tasks tasks
	  
	  options = tasks.options
	  options ||= {}
	  
	  @options = {
		:compilemode => ENV['config'].nil? ? "Debug" : ENV['config'],
		:clrversion => 'v4.0.30319',
		:platform => 'x86',
		:unit_test_list_file => 'TESTS.txt',
		:unit_test_projects => [],
		:source => 'src'}.merge(options)

	  tasks.clean ||= []

	  enable_docs tasks
	  make_assembly_info tasks
	  enable_ripple tasks
	  make_clean tasks
	  make_compile tasks
	  make_unit_test tasks

	  if tasks.defaults != nil
		@defaultTask.enhance tasks.defaults
	  end
	  
	  if tasks.ci_steps != nil
		@ciTask.enhance tasks.ci_steps
	  end
	  
	end
	
	
	def make_default_tasks(tasks)
	  @defaultTask = Rake::Task.define_task :default do
	  
	  end
	  
	  @defaultTask.add_description "**Default**, compiles and runs tests"
	  
	  @ciTask = Rake::Task.define_task :ci do
	  
	  end
	  
	  @ciTask.add_description "Target used for the CI server"
	  @ciTask.enhance [:default]
	end
	
	
	def make_clean(tasks)
	  if tasks.clean.any?
	    @cleanTask = Rake::Task.define_task :clean do
		  tasks.clean.each do |dir|
			cleanDirectory dir
		  end
		end
	  
		@cleanTask.add_description "Prepares the working directory for a new build"
	  end
	end
	
	def make_assembly_info(tasks)
	  if tasks.assembly_info != nil
	    @versionTask = Rake::Task.define_task :version do
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
		
		@versionTask.add_description "Update the version information for the build"
	  end
	end

	def make_compile(tasks)
	  if tasks.compile != nil
		@compileTask = Rake::Task.define_task :compile do
		  MSBuildRunner.compile @options.merge(tasks.compile)
		end
		
		@compileTask.add_description "Compiles the application"
		
		if (tasks.clean.any?)
			@compileTask.enhance [:clean]
		end

		if (tasks.assembly_info != nil)
			@compileTask.enhance [:version]
		end
		
		if (tasks.ripple_enabled)
			@compileTask.enhance ["ripple:restore"]
		end 
		
		@defaultTask.enhance [:compile]
	  end
	end
	
	
	def make_unit_test(tasks)
	  if @options[:unit_test_projects].any?
		@nunitTask = Rake::Task.define_task :unit_test do
		  runner = NUnitRunner.new @options
		  runner.executeTests options[:unit_test_projects]
		end
	  elsif @options[:unit_test_list_file] != nil
		file = @options[:unit_test_list_file]
	  
		@nunitTask = Rake::Task.define_task :unit_test do
		  runner = NUnitRunner.new @options
		  runner.executeTestsInFile file
		end
	  end
	  
	  if @nunitTask != nil
		@nunitTask.enhance [:compile]
		@nunitTask.add_description "Runs unit tests"
		
		@defaultTask.enhance [:unit_test]
	  end
	end
	
	def enable_ripple(tasks)
	  if tasks.ripple_enabled
	    require File.join(File.dirname(__FILE__), 'ripple')
		
		tasks.clean << 'artifacts'
		
		#TODO -- add more stuff in to tasks
	  end
	end
	
	def enable_docs(tasks)
	  if tasks.fubudocs_enabled
		require File.join(File.dirname(__FILE__), 'fubudocs')
	  end
	end
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
  puts 'Cleaning directory ' + dir
  FileUtils.rm_rf dir;
  waitfor { !exists?(dir) }
  Dir.mkdir dir
end

def cleanFile(file)
  File.delete file unless !File.exist?(file)
end