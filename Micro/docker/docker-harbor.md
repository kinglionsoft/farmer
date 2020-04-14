# docker 私有仓库

## 安装Harbor

* 参考： https://jaminzhang.github.io/docker/Enterprise-class-private-Docker-Container-Registry-Harbor-deploying/
* Hostname: registry.local.com
* 安装docker-compose: https://docs.docker.com/compose/install/#install-compose
```bash
#下载 https://storage.googleapis.com/harbor-releases/harbor-offline-installer-v1.5.4.tgz
#解压到 /opt

# ssl cert

openssl genrsa -out ca.key 4096
openssl req -x509 -new -nodes -sha512 -days 3650 \
    -subj "/C=CN/ST=SC/L=CD/O=Eson/OU=Eson/CN=registry.local.com" \
    -key ca.key \
    -out ca.crt

openssl genrsa -out registry.local.com.key 4096

openssl req -sha512 -new \
    -subj "/C=CN/ST=SC/L=CD/O=Eson/OU=Eson/CN=registry.local.com" \
    -key registry.local.com.key \
    -out registry.local.com.csr 

cat > v3.ext <<-EOF
authorityKeyIdentifier=keyid,issuer
basicConstraints=CA:FALSE
keyUsage = digitalSignature, nonRepudiation, keyEncipherment, dataEncipherment
extendedKeyUsage = serverAuth 
subjectAltName = @alt_names

[alt_names]
DNS.1=registry.local.com
DNS.2=yx
DNS.3=registry
EOF

openssl x509 -req -sha512 -days 3650 \
    -extfile v3.ext \
    -CA ca.crt -CAkey ca.key -CAcreateserial \
    -in registry.local.com.csr \
    -out registry.local.com.crt

cp registry.local.com.crt /data/cert/
cp registry.local.com.key /data/cert/     

openssl x509 -inform PEM -in registry.local.com.crt -out registry.local.com.cert
mkdir -p /etc/docker/certs.d/registry.local.com/
cp registry.local.com.cert /etc/docker/certs.d/registry.local.com/
cp registry.local.com.key /etc/docker/certs.d/registry.local.com/
cp ca.crt /etc/docker/certs.d/registry.local.com/

```

* nginx
由于 80 端口已被使用， 删除 ./common/config/nginx/nginx.conf 中的80 跳转

* 启停

``` bash
cd /opt/harbor
# 停
sudo docker-compose stop
# 启
sudo docker-compose start
# 变更配置
sudo docker-compose down -v
vim harbor.cfg
sudo prepare
sudo docker-compose up -d
# 清理
sudo docker-compose down -v
rm -rf /data/database /data/registry
```

## 将registry.local.com的自签名证书加入受信列表

* Linux： https://docs.docker.com/registry/insecure/#use-self-signed-certificates

``` bash
curl -s -L http://share.local.com/import-registry.local.com-ca-root.sh | bash
```

> 若无效，参考手动添加：https://askubuntu.com/questions/440580/how-does-one-remove-a-certificate-authoritys-certificate-from-a-system

* Windows: 下载 http://share.local.com/registry.local.com_ca_root.crt，安装到 【本地计算机】 -> 【受信任的根证书颁发机构】


## Errors

* docker login resitry.local.com : Cannot autolaunch D-Bus without X11 $DISPLAY. 参照： https://github.com/docker/compose/issues/6023#issuecomment-419792269

``` bash

```