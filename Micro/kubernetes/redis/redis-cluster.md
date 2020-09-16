# Redis Cluster

``` bash
# 创建一个临时的pod
kubectl run -i --tty ubuntu --image=ubuntu:bionic /bin/bash

# 更改国内原
tee /etc/apt/sources.list <<EOF
deb http://mirrors.aliyun.com/ubuntu/ bionic main restricted universe multiverse
deb http://mirrors.aliyun.com/ubuntu/ bionic-security main restricted universe multiverse
deb http://mirrors.aliyun.com/ubuntu/ bionic-updates main restricted universe multiverse
deb http://mirrors.aliyun.com/ubuntu/ bionic-proposed main restricted universe multiverse
deb http://mirrors.aliyun.com/ubuntu/ bionic-backports main restricted universe multiverse
deb-src http://mirrors.aliyun.com/ubuntu/ bionic main restricted universe multiverse
deb-src http://mirrors.aliyun.com/ubuntu/ bionic-security main restricted universe multiverse
deb-src http://mirrors.aliyun.com/ubuntu/ bionic-updates main restricted universe multiverse
deb-src http://mirrors.aliyun.com/ubuntu/ bionic-proposed main restricted universe multiverse
deb-src http://mirrors.aliyun.com/ubuntu/ bionic-backports main restricted universe multiverse
EOF

apt update && apt install -y redis-tools dnsutils net-tools python3.6 python-pip

pip install -i https://pypi.tuna.tsinghua.edu.cn/simple redis-trib

redis-trib.py create \
  `dig +short redis-0.redis.default.svc.cluster.local`:6379 \
  `dig +short redis-1.redis.default.svc.cluster.local`:6379 \
  `dig +short redis-2.redis.default.svc.cluster.local`:6379

redis-trib.py replicate \
  --master-addr `dig +short redis-0.redis.default.svc.cluster.local`:6379 \
  --slave-addr `dig +short redis-3.redis.default.svc.cluster.local`:6379
redis-trib.py replicate \
  --master-addr `dig +short redis-1.redis.default.svc.cluster.local`:6379 \
  --slave-addr `dig +short redis-4.redis.default.svc.cluster.local`:6379
redis-trib.py replicate \
  --master-addr `dig +short redis-2.redis.default.svc.cluster.local`:6379 \
  --slave-addr `dig +short redis-5.redis.default.svc.cluster.local`:6379

```