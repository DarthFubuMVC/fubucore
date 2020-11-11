require 'win32/registry'

COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
BUILD_VERSION = '100.0.0'
tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"
BUILD_NUMBER = build_number 
CLR_VERSION = "v4.0.30319"
MSBUILD_TOOLSVERSION = "dotnetcli"

load 'BuildUtils.rb'

desc 'Compile the code'
task :compile => [:clean, :version] do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'src/FubuCore.sln', :clrversion => CLR_VERSION, :extraSwitches => ["p:WarningLevel=1"], :msbuildToolsVersion => MSBUILD_TOOLSVERSION
end

desc 'Run the unit tests'
task :test => [:compile] do
  sh "dotnet test src/FubuCore.sln"
end

desc "Prepares the working directory for a new build"
task :clean do
  FileUtils.rm_rf 'artifacts'
  Dir.mkdir 'artifacts'
end


desc "Update the version information for the build"
task :version do
  asm_version = build_number
  
  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  puts "##teamcity[buildNumber '#{build_number}']" unless tc_build_number.nil?
  puts "Version: #{build_number}" if tc_build_number.nil?
  
  options = {
    :description => 'FubuCore',
    :product_name => 'FubuCore',
    :copyright => 'Copyright 2008-2013 Jeremy D. Miller, Josh Arnold, Joshua Flanagan, et al. All rights reserved.',
    :trademark => commit,
    :version => asm_version,
    :file_version => build_number,
    :informational_version => asm_version
  }
  
  puts "Writing src/CommonAssemblyInfo.cs..."
  File.open('src/CommonAssemblyInfo.cs', 'w') do |file|
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