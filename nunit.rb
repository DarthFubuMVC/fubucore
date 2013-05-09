module FubuRake
  class NUnit
    def self.create_task(tasks, options)
	  nunitTask = nil
	  if options[:unit_test_projects].any?
		nunitTask = Rake::Task.define_task :unit_test do
		  runner = NUnitRunner.new options
		  runner.executeTests options[:unit_test_projects]
		end
	  elsif options[:unit_test_list_file] != nil
		file = options[:unit_test_list_file]
	  
		nunitTask = Rake::Task.define_task :unit_test do
		  runner = NUnitRunner.new options
		  runner.executeTestsInFile file
		end
	  end
	  
	  if nunitTask != nil
		nunitTask.enhance [:compile]
		nunitTask.add_description "Runs unit tests"
	  end
	  
	  return nunitTask
	end
  end
end

class NUnitRunner
	include FileTest

	def initialize(paths)
		@sourceDir = paths.fetch(:source, 'src')
		@resultsDir = paths.fetch(:results, 'results')
		@compilePlatform = paths.fetch(:platform, 'x86')
		@compileTarget = paths.fetch(:compilemode, 'debug')
	puts "COMPILE TARGET IS #{@compilePlatform}"
		@nunitExe = Nuget.tool("NUnit", "nunit-console#{(@compilePlatform.empty? ? '' : "-#{@compilePlatform}")}.exe") + Platform.switch("nothread")
	end
	
	def executeTests(assemblies)
		Dir.mkdir @resultsDir unless exists?(@resultsDir)
		
		assemblies.each do |assem|
			file = File.expand_path("#{@sourceDir}/#{assem}/bin/#{@compileTarget}/#{assem}.dll")
			sh Platform.runtime("#{@nunitExe} -xml=#{@resultsDir}/#{assem}-TestResults.xml \"#{file}\"")
		end
	end
	
	def executeTestsInFile(file)
	  if !File.exist?(file)
		throw "File #{file} does not exist"
	  end
	  
	  tests = Array.new

	  file = File.new(file, "r")
	  assemblies = file.readlines()
	  assemblies.each do |a|
		test = a.gsub("\r\n", "").gsub("\n", "")
		tests.push(test)
	  end
	  file.close
	  
	  if (!tests.empty?)
	    executeTests tests
	  end
	end
end