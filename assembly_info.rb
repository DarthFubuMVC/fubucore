module FubuRake
  class AssemblyInfo
    def self.create tasks, options
	  if tasks.assembly_info == nil
	   return nil
	  end
	  
	    versionTask = Rake::Task.define_task :version do
		    begin
			  commit = `git log -1 --pretty=format:%H`
		    rescue
			  commit = "git unavailable"
		    end
		    puts "##teamcity[buildNumber '#{options[:build_number]}']" unless options[:tc_build_number].nil?
		    puts "Version: #{options[:build_number]}" if options[:tc_build_number].nil?
			
			options = {
				:trademark => commit, 
				:product_name => 'CHANGEME', 
				:description => options[:build_number], 
				:version => options[:asm_version], 
				:file_version => options[:build_number],
				:informational_version => options[:asm_version],
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
		
		versionTask.add_description "Update the version information for the build"
	
		return versionTask
	end
  end
end