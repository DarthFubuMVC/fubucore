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
