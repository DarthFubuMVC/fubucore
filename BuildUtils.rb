include Rake::DSL
require 'erb'
require 'win32/registry'

class MSBuildRunner
	def self.compile(attributes)
		compileTarget = attributes.fetch(:compilemode, 'debug')
		solutionFile = attributes[:solutionfile]
		
		attributes[:projFile] = solutionFile
	  attributes[:properties] = ["Configuration=#{compileTarget}"]
    buildTarget = attributes.fetch(:buildTarget, 'rebuild')
	  attributes[:extraSwitches] = ["maxcpucount", "v:m", "t:#{buildTarget}"] | attributes.fetch(:extraSwitches, [])

		self.runProjFile(attributes)
	end

	def self.runProjFile(attributes)
		version = attributes.fetch(:clrversion, 'v4.0.30319')
		compileTarget = attributes.fetch(:compilemode, 'debug')
	  projFile = attributes[:projFile]

	  msbuildToolsVersion = attributes.fetch(:msbuildToolsVersion, "14.0")
		puts "Using configured MSBuildToolsVersion #{msbuildToolsVersion} (or else it didn't have any version and this was the default)"
			
		msbuildFilePath = "dotnet msbuild"
		unless msbuildToolsVersion == "dotnetcli"
			msbuildFilePath = "\"#{getMsbuildToolsPath(msbuildToolsVersion)}msbuild.exe\""
		end

		properties = attributes.fetch(:properties, [])

		switchesValue = ""

		properties.each do |prop|
			switchesValue += "/property:#{prop} "
		end

		extraSwitches = attributes.fetch(:extraSwitches, [])
		extraSwitches.each do |switch|
			switchesValue += "/#{switch} "
		end

		targets = attributes.fetch(:targets, [])
		targetsValue = "";
		targets.each do |target|
			targetsValue += "/t:#{target} "
		end

		sh "#{msbuildFilePath} #{projFile} #{targetsValue} #{switchesValue}"
	end


	def self.getMsbuildToolsPath(msbuildToolsVersion)
		# Try the old way to find MSBuild 14.0
		Win32::Registry::HKEY_LOCAL_MACHINE.open("Software\\Microsoft\\MSBuild\\ToolsVersions") do |reg|
			reg.each_key do |k,v|
				next unless (k.downcase == msbuildToolsVersion.downcase)
				reg.open(k) do |subkey|
					return subkey['MSBuildToolsPath']
				end
			end
		end

		puts "Didn't find MSBuild 14 in the registry. Attempting to find MSBuild 15 in Visual Studio install folder"

		# Try the new way with VS2017 and VS2019 to find MSBuild 15
		vs_install_path = `"C:\\Program Files (x86)\\Microsoft Visual Studio\\Installer\\vswhere.exe" -latest -products * -requires Microsoft.Component.MSBuild -property installationPath`
		puts "Visual Studio install path found: #{vs_install_path}"
		vs_install_path = vs_install_path.chomp
		return "#{vs_install_path}\\MSBuild\\15.0\\Bin\\"
	end
end

class AsmInfoBuilder
	def initialize(buildnumber, properties)
		@properties = properties;

		@properties['Version'] = @properties['InformationalVersion'] = buildnumber;
	end



	def write(file)
		template = %q{
using System;
using System.Reflection;
using System.Runtime.InteropServices;

<% @properties.each {|k, v| %>
[assembly: Assembly<%=k%>Attribute("<%=v%>")]
<% } %>
		}.gsub(/^    /, '')

	  erb = ERB.new(template, 0, "%<>")

	  File.open(file, 'w') do |file|
		  file.puts erb.result(binding)
	  end
	end
end

class InstallUtilRunner
	def installServices(services, parameters)
		services.each do |service|
			params = ""
			parameters.each_pair {|key, value| params = params + "/" + key + "=" + value + " "}
			sh "tools/installutil /i #{params} #{service}"
		end
	end
end