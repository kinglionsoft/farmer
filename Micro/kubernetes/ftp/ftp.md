# FTP on K8S

## 需求

* 文件存储服务上线前，兼容目前版本的临时方案

## 安装

* 创建Docker镜像

``` bash
docker build -t registry.local.com/ctc/ftp .

# test
docker run --rm -i -t -v /data/ftp:/home/vsftpd  -p 20:20 -p 21:21 -p 9000-9010:9000-9010 \
                -e FTP_USER=ctc \
                -e FTP_PASS=ctc \
                -e PASV_ADDRESS=172.16.0.20 \
                -e PASV_MIN_PORT=9000 \
                -e PASV_MAX_PORT=9010 \
                --name ftp registry.local.com/ctc/ftp
```

* 部署FTP

``` bash
kubectl apply -f ftp.yaml
```

## 使用
