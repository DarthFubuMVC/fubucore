require 'fuburake'


FubuRake::Solution.new do |sln|
	sln.assembly_info = {
		:product_name => "FubuCore",
		:copyright => 'Copyright 2008-2013 Jeremy D. Miller, Josh Arnold, Joshua Flanagan, et al. All rights reserved.'
	}
	
	sln.ripple_enabled = true
	sln.fubudocs_enabled = true
end


