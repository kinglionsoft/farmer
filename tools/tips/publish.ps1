Remove-Item dist -Recurse -ErrorAction Ignore
mkdir .\dist
Copy-Item .\css .\dist\ -Recurse
Copy-Item .\images .\dist\ -Recurse
Copy-Item .\js .\dist\ -Recurse
Copy-Item .\bower_components\dom-to-image\dist\dom-to-image.min.js .\dist\js\
Copy-Item .\bower_components\weui\dist\style\weui.css .\dist\css\
Copy-Item .\index.html .\dist\
(Get-Content .\dist\index.html -Encoding UTF8).Replace('bower_components/dom-to-image/dist/', 'js/').Replace("bower_components/weui/dist/style/", "css/") | Set-Content .\dist\index.html -Encoding UTF8