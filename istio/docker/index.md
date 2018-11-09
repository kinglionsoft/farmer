# docker

## 本地仓促

### 部署
* IP: 192.168.0.149
* OS: Ubuntu 18.04.1 LTS

``` bash
sudo -i

# add user
adduser yc
usermod -aG sudo yc

# install docker
source install-docker.sh

# local registry
docker pull registry
mkdir /var/lib/docker/registry
docker run -d -p 5000:5000 \
    --name docker-registry \
    --restart=always \
    -v /var/lib/docker/registry:/var/lib/registry \
    registry

```

## 使用本地仓储
客户机使用本地仓储前，需要将本地仓储添加到受信列表后重启docker.

* Ubuntu 18.04.1 LTS

``` bash
# /etc/default/docker
DOCKER_OPTS='--insecure-registry 192.168.0.149:5000'
```
* docker 添加私有registry
写入：/etc/docker/daemon.json

``` js
{
  "registry-mirrors": ["https://78i6a9m9.mirror.aliyuncs.com"],
  "insecure-registries": ["192.168.0.240:5000"]
}

* Centos

``` bash
# /etc/docker/daemon.json
{
    "insecure-registries" : ["192.168.0.149:5000"]
}
```

* Windows: Settings -> Daemon
