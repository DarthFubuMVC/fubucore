COMPILE_TARGET = ENV['config'].nil? ? "Debug" : ENV['config']
CLR_TOOLS_VERSION = "v4.0.30319"

load 'fuburake.rb'

load "VERSION.txt"

ARTIFACTS = File.expand_path("artifacts")

FubuRake::Solution.new do |sln|
	sln.compile = {
		:compilemode => COMPILE_TARGET, 
		:solutionfile => 'src/FubuCore.sln', 
		:clrversion => CLR_TOOLS_VERSION
	}
				 
	sln.assembly_info = {
		:product_name => "FubuCore",
		:copyright => 'Copyright 2008-2013 Jeremy D. Miller, Josh Arnold, Joshua Flanagan, et al. All rights reserved.'
	}
	
	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
end


desc "**Default**, compiles and runs tests"
task :default => [:compile, :unit_test]

desc "Target used for the CI server"
task :ci => [:default, :history, :package]

desc "Runs unit tests"
task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET, :source => 'src', :platform => 'x86'
  runner.executeTestsInFile 'TESTS.txt'
end
