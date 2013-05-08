namespace :ripple do
	desc "Restores nuget package files"
	task :restore do
	  puts 'Restoring all the nuget package files'
      sh 'ripple restore'
	end

	desc "Updates nuget package files to the latest"
	task :update do
	  puts 'Updating all the nuget package files'
	  sh 'ripple update'
	end

	desc "creates a history file for nuget dependencies"
	task :history do
	  sh 'ripple history'
	end
	
	desc "publishes all the nuget's published by this solution"
	task :publish do
	  nuget_api_key = ENV['apikey']
	  server = ENV['server']
	  cmd = "ripple publish #{BUILD_NUMBER} #{nuget_api_key}"
	  cmd = cmd + " --server #{server}" unless server.nil?
	  sh cmd
	end
	
	desc "packages the nuget files from the nuspec files in packaging/nuget and publishes to /artifacts"
	task :package => [:history] do
		COMPILE_TARGET = 'release'
		Rake::Task["compile"].execute
	
		sh "ripple local-nuget --version #{BUILD_NUMBER} --destination artifacts"
	end
end