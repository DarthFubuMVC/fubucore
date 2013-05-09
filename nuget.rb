module Nuget
  def self.tool(package, tool)
    nugetDir = Dir.glob(File.join(package_root,"#{package}*")).sort.last
    return File.join(nugetDir, "tools", tool) if File.directory?(nugetDir)
        
    
    File.join(Dir.glob(File.join(package_root,"#{package}.[0-9]*")).sort.last, "tools", tool)
  end
  
  def self.package_root
    root = nil
    
    packroots = Dir.glob("{source,src}/packages")

    return packroots.last if packroots.length > 0

    Dir.glob("{source,src}").each do |d|
      packroot = File.join d, "packages"
      FileUtils.mkdir_p(packroot) 
      root = packroot
    end       

    root
  end
end
