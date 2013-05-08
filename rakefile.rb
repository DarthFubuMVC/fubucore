load 'fuburake.rb'
load "VERSION.txt"

FubuRake::Solution.new do |sln|
	sln.compile = {
		:solutionfile => 'src/FubuCore.sln'
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
