#!/bin/bash

# root 运行
# $1: ssr client ip

echo 'install polipo'
apt install -y polipo

cat <<EOF > /etc/polipo/config 
logSyslog = false
logFile = "/var/log/polipo/polipo.log"

socksParentProxy = "$1:1080"
socksProxyType = socks5

chunkHighMark = 50331648
objectHighMark = 16384

serverMaxSlots = 64
serverSlots = 16
serverSlots1 = 32

proxyAddress = "0.0.0.0"
proxyPort = 8123

EOF

echo 'restart polipo'
service polipo restart

echo 'test http proxy'
export http_proxy="http://127.0.0.1:8123/"
export https_proxy="http://127.0.0.1:8123/"
http_status = `curl -I -m 10 -o /dev/null -s -w %{http_code}  https://www.google.com.hk`
if ["$http_status"x = "200"x]; then
	echo 'http proxy succeeded'
else
	echo 'http proxy failed'
fi	

