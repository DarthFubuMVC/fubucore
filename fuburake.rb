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
	  
	  @defaultTask = create_task(:default, "**Default**, compiles and runs tests")
	  @ciTask = create_task(:ci,  "Target used for the CI server")
	  @ciTask.enhance [:default]
	  
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
	  @versionTask = FubuRake::AssemblyInfo.create tasks, @options
	  enable_ripple tasks
	  make_clean tasks
	  @compileTask = FubuRake::MSBuild.create_task tasks, @options
	  @nunitTask = FubuRake::NUnit.create_task tasks, @options

	  if @compileTask != nil
		@compileTask.enhance [:clean] unless @cleanTask == nil
		@compileTask.enhance [:version] unless @versionTask == nil
		if tasks.ripple_enabled 
		  @compileTask.enhance ["ripple:restore"]
		end
		
		@nunitTask.enhance [:compile] unless @nunitTask == nil
	  end
	  
	  
	  
	  @defaultTask.enhance [:compile] unless @compileTask == nil
	  @defaultTask.enhance [:unit_test] unless @nunitTask == nil
	  
	  if tasks.defaults != nil
		@defaultTask.enhance tasks.defaults
	  end
	  
	  if tasks.ci_steps != nil
		@ciTask.enhance tasks.ci_steps
	  end
	  
	end
	
	def create_task(name, description)
	  task = Rake::Task.define_task name do
	  
	  end
	  task.add_description description
	  
	  return task
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