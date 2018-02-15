Pod::Spec.new do |s|
  s.name         = "MapsyncLib"
  s.version      = "0.0.3"
  s.summary      = "A short description of MapsyncLib."

  s.description  = <<-DESC
  beta test pod.  
                   DESC

  s.homepage     = "http://mapsync.io"
  s.license      = "MIT"
  s.author       = { "Jaeyong Sung" => "jae@mapsync.io" }
  s.platform     = :ios, "11.0"
  s.source            = { :git => 'https://amakermark@bitbucket.org/amakermark/mapsynclib.git', :tag => 'v0.1.0' }
  s.ios.deployment_target = '11.0'
  s.ios.vendored_frameworks = 'MapsyncLib.framework'
  s.exclude_files = "Classes/Exclude"

  s.dependency "AWSS3", "~> 2.6.10"
  s.dependency "Alamofire", "~> 4.6.0"
  s.dependency "SwiftyJSON", "~> 4.0.0"
  s.dependency "SwiftHash"

end