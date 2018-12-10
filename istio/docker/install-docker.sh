#!/bin/bash
apt remove docker docker-engine docker.io
apt update
apt install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
add-apt-repository \
   "deb [arch=amd64] https://download.docker.com/linux/ubuntu \
   $(lsb_release -cs) \
   stable"
apt update
apt install -y docker-ce

# 配置镜像
cat << EOF > /etc/docker/daemon.json
{
    "insecure-registries" : ["registry.yx.com:443"],
    "registry-mirrors": ["https://registry.docker-cn.com"]
}
EOF

# 配置代理
cat << EOF > /etc/systemd/system/docker.service.d/http-proxy.conf
[Service]
Environment="HTTPS_PROXY=http://192.168.0.237:8123" "NO_PROXY=localhost,127.0.0.1,registry.docker-cn.com,registry.hub.docker.com,registry.yx.com"
EOF

# test docker
docker run hello-world