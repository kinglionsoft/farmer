# Ubuntu Server 18.01 配置优化

``` bash

# 设置时区
tzselect
sudo rm /etc/localtime && sudo ln -s /usr/share/zoneinfo/Asia/Shanghai  /etc/localtime

# 临时增加打开文件数量限制 - max_file_open
ulimit -n 65535

# 永久增加打开文件数量限制 - max_file_open
echo "* hard nofile 100000
* soft nofile 100000
root hard nofile 100000
root soft nofile 100000">>/etc/security/limits.conf

# 增加可用端口数量
echo  "10000    65500" >  /proc/sys/net/ipv4/ip_local_port_range

# 临时文件清理策略
# /usr/lib/tmpfiles.d/tmp.conf
D /tmp 1777 root root 5d
# Windows => Start > Settings > System > Storage > Toggle on Storage sense

# 硬盘扩容
fdisk -l

parted

p

resizepart 2

100%

q

resize2fs /dev/sda2
```
