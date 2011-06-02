namespace :nug do
	@nuget = "lib/nuget.exe"
	@nugroot = File.expand_path("/nugs")
	@dependencies = []
	
	desc "Build the nuget package"
	task :build do
		sh "#{@nuget} pack packaging/nuget/fubucore.nuspec -o #{ARTIFACTS} -Version #{BUILD_NUMBER} -Symbols"
		sh "#{@nuget} pack packaging/nuget/fubulocalization.nuspec -o #{ARTIFACTS} -Version #{BUILD_NUMBER} -Symbols"
		sh "#{@nuget} pack packaging/nuget/fubutestingsupport.nuspec -o #{ARTIFACTS} -Version #{BUILD_NUMBER} -Symbols"
	end
	
	desc "pulls new NuGet updates from your local machine"
	task :pull, [:location] => [:build] do |t, args|
		args.with_defaults(:location => 'local')
		location = args[:location]
		
		@dependencies.each do |f|
			#nuget install
		end
	end
		
	desc "pushes new NuGet udates to your local machine"
	task :push, [:location] => [:build] do |t, args|
		args.with_defaults(:location => 'local')
		location = args[:location]
		
		FileUtils.makedirs(@nugroot)
		
		Dir["#{ARTIFACTS}/*.nupkg"].each do |fn|
			puts "Copying package #{fn} to #{@nugroot}"
			FileUtils.cp fn, @nugroot
		end
	end
	
end