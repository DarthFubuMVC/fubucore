include FileUtils
include FileTest

require File.join(File.dirname(__FILE__), 'nunit')
require File.join(File.dirname(__FILE__), 'msbuild')
require File.join(File.dirname(__FILE__), 'nuget')
require File.join(File.dirname(__FILE__), 'platform')
require File.join(File.dirname(__FILE__), 'ripple')
require File.join(File.dirname(__FILE__), 'assembly_info')

load "VERSION.txt"

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
	  
	  tc_build_number = ENV["BUILD_NUMBER"]
	  build_revision = tc_build_number || Time.new.strftime('5%H%M')
	  asm_version = BUILD_VERSION + ".0"
	  build_number = "#{BUILD_VERSION}.#{build_revision}"
	  
	  @options = {
		:compilemode => ENV['config'].nil? ? "Debug" : ENV['config'],
		:clrversion => 'v4.0.30319',
		:platform => 'x86',
		:unit_test_list_file => 'TESTS.txt',
		:unit_test_projects => [],
		:build_number => build_number,
		:asm_version => asm_version,
		:tc_build_number => tc_build_number,
		:build_revision => build_revision,
		:source => 'src'}.merge(options)

	  tasks.clean ||= []

	  enable_docs tasks
	  make_assembly_info tasks
	  enable_ripple tasks
	  make_clean tasks
	  make_compile tasks
	  make_unit_test tasks

	  if @compileTask != nil
		@compileTask.enhance [:clean] unless @cleanTask == nil
		@compileTask.enhance [:version] unless @versionTask == nil
		if tasks.ripple_enabled 
		  @compileTask.enhance ["ripple:restore"]
		end
	  end
	  
	  @defaultTask.enhance [:compile] unless @compileTask == nil
	  
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
	  @versionTask = FubuRake::AssemblyInfo.create tasks, @options
	end

	def make_compile(tasks)
	  @compileTask = FubuRake::MSBuild.create_task tasks, @options
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
	    FubuRake::Ripple.create tasks, @options
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