#!/bin/bash
sudo apt remove docker docker-engine docker.io
sudo apt update
sudo apt install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo apt-key add -
sudo add-apt-repository \
   "deb [arch=amd64] https://download.docker.com/linux/ubuntu \
   $(lsb_release -cs) \
   stable"
sudo apt update
sudo apt install -y docker-ce

sudo tee /etc/docker/daemon.json <<EOF
{
    "insecure-registries" : ["registry.local.com:443"],
    "registry-mirrors": ["https://registry.docker-cn.com"]
}
EOF

sudo mkdir -p /etc/systemd/system/docker.service.d
sudo tee /etc/systemd/system/docker.service.d/http-proxy.conf << EOF
[Service]
Environment="HTTPS_PROXY=http://192.168.1.123:11080" "NO_PROXY=localhost,127.0.0.1,.docker-cn.com,.docker.com,.local.com,.docker.io,.aliyuncs.com"
EOF

sudo systemctl daemon-reload && sudo systemctl restart docker
# test docker
sudo docker run hello-world