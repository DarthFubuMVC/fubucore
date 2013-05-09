load 'fuburake.rb'

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
	
	#sln.defaults = [:fake]
	#sln.ci_steps = [:fake]
end

desc "fake task"
task :fake do
  puts "I AM THE FAKE TASK!"
end
