COMPILE_TARGET = ENV['config'].nil? ? "Debug" : ENV['config']
CLR_TOOLS_VERSION = "v4.0.30319"



load 'fuburake.rb'



include FileTest
require 'albacore'
load "VERSION.txt"

RESULTS_DIR = "results"
PRODUCT = "FubuCore"
COPYRIGHT = 'Copyright 2008-2013 Jeremy D. Miller, Josh Arnold, Joshua Flanagan, et al. All rights reserved.';
COMMON_ASSEMBLY_INFO = 'src/CommonAssemblyInfo.cs';

@teamcity_build_id = "bt396"
tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
#BUILD_NUMBER = "#{BUILD_VERSION}.#{build_revision}"
ARTIFACTS = File.expand_path("artifacts")

props = { :stage => File.expand_path("build"), :artifacts => ARTIFACTS }


FubuRake::Solution.new do |sln|
	sln.clean = [props[:stage], props[:artifacts]]
	
	sln.compile = {:compilemode => COMPILE_TARGET, 
                 :solutionfile => 'src/FubuCore.sln', 
				 :clrversion => CLR_TOOLS_VERSION}
				 
	sln.assembly_info = {
		:product_name => PRODUCT,
		:copyright => COPYRIGHT
	}
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
