module FubuRake
  class Ripple
	def self.create(tasks, options)
	  if !tasks.ripple_enabled
		return
	  end
	
	  tasks.clean << 'artifacts'
	
	  restoreTask = Rake::Task.define_task 'ripple:restore' do
	    puts 'Restoring all the nuget package files'
        sh 'ripple restore'
	  end
	  restoreTask.add_description "Restores nuget package files and updates all floating nugets"
	
	  updateTask = Rake::Task.define_task 'ripple:update' do
	  puts 'Cleaning out existing packages out of paranoia'
		sh 'ripple clean'
		
		puts 'Updating all the nuget package files'
		sh 'ripple update'
	  end
	  updateTask.add_description  "Updates nuget package files to the latest"
	  
	  
	  historyTask = Rake::Task.define_task 'ripple:history' do
	    sh 'ripple history'
	  end
	  historyTask.add_description "creates a history file for nuget dependencies"
	
	  packageTask = Rake::Task.define_task 'ripple:package' do
	    sh "ripple local-nuget --version #{options[:build_number]} --destination artifacts"
	  end
	  packageTask.add_description "packages the nuget files from the nuspec files in packaging/nuget and publishes to /artifacts"
	end
  end
end

