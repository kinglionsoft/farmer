# MaxScale 搭建高可用
MaxScale具备读写分离、负载均衡、主从切换等功能，所以只需要保证MaxScale节点高可用即可。

## MaxScale

* 搭建2台MaxScale服务器，略。

## K8S中访问MaxScale

### Service + Endpoint
* [services-without-selectors](https://kubernetes.io/docs/concepts/services-networking/service/#services-without-selectors)
* Endpoint 直接指向MaxScale节点
* 优点：轻量高效，无第三方依赖
* 缺点：Endpoint不支持健康检查，https://github.com/kubernetes/kubernetes/issues/77738

### Keeplived
* Keeplived 接管MaxScale的连接。
* Endpoint 指向Keeplived的虚拟IP，弥补Endpoint不支持健康检查的问题。


#### 安装Keeplived

* 安装
```bash
# CentOS
dnf install keepalived -y
```

* create mariadb user

```
create user keepalived@localhost identified by '1234';
grant replication client on *.* to keepalived@localhost;
flush privileges;

```

* 非阿里云VPC环境， 非抢占式配置 /etc/keepalived/keepalived.conf

```
! Configuration File for keepalived

global_defs {
   notification_email {
     acassen@firewall.loc
     failover@firewall.loc
     sysadmin@firewall.loc
   }
   notification_email_from Alexandre.Cassen@firewall.loc
   smtp_server 192.168.200.1
   smtp_connect_timeout 30
   router_id dbslave1
   vrrp_skip_check_adv_addr
   #vrrp_strict
   vrrp_garp_interval 0
   vrrp_gna_interval 0
   enable_script_security
   script_user root
}

vrrp_script  check_mariadb {
  script "/etc/keepalived/check_mariadb.sh"  
  interval 2  
  weight -20
}

vrrp_instance VI_1 {
  state  BACKUP   # 非抢占模式
  interface eth0    # 设置实例绑定的网卡
  virtual_router_id 51    # 同一实例下virtual_router_id必须相同
  priority 100
  advert_int 1
  nopreempt             # 非抢占模式
  authentication {
    auth_type PASS
    auth_pass 123456
  }
  track_script {
    check_mariadb
  }
  virtual_ipaddress {
    172.16.153.10     # 可以多个虚拟IP，换行即可
  }

  # 单播方式更搞笑
  unicast_src_ip 172.16.153.178
  unicast_peer {
    172.16.153.177
  }
}
```

* 阿里云VPC

```
https://help.aliyun.com/document_detail/110067.html?spm=a2c4g.11186623.6.618.74915d1azFVTyT
```

* /etc/keepalived/check_mariadb.sh

```bash
#!/bin/bash
mysql_bin=/usr/bin/mysql
user=keepalived
pw=1234
host=127.0.0.1
port=3306
# Seconds_Behind_Master
sbm=60

io_thread_state=`$mysql_bin -h $host -P $port -u$user -p$pw -e 'show slave status\G'  2>/dev/null|grep 'Slave_IO_Running:'|awk '{print $NF}'`
echo "io_thread_state: $io_thread_state"

sql_thread_state=`$mysql_bin -h $host -P $port -u$user -p$pw -e 'show slave status\G' 2>/dev/null|grep 'Slave_SQL_Running:'|awk '{print $NF}'`
echo "sql_thread_state: $sql_thread_state"

SBM=`$mysql_bin -h $host -P $port -u$user -p$pw -e 'show slave status\G' 2>/dev/null|grep 'Seconds_Behind_Master:'|awk '{print $NF}'`
echo "Seconds_Behind_Master: $SBM"

#Check for $mysql_bin
if [ ! -f $mysql_bin ];then
    echo 'the path of mysqlbin is incorrect,please check msyql path'
    exit 2
fi


# check mysql status whether is dead
service mariadb status &>/dev/null
if [ $? -ne 0 ];then
    systemctl stop keepalived
    echo 'mysql is dead'
    exit 1
fi

# -z 表示如果$IOThread变量为空，说明数据库服务不可用，已down
if [[ -z "$io_thread_state" ]];then
		systemctl stop keepalived
		echo 'mysql is dead'
    exit 1
fi


if [[ "$io_thread_state" == "Connecting" && "$sql_thread_state" == "Yes" ]];then
		echo 'master is down,but slave still works'
    exit 0

    # Seconds_Behind_Master timeout
    elif [[ $SBM -ge $sbm ]];then
        systemctl stop keepalived
        exit 1
    else
        exit 0
fi
```

* keepalived.service

```
[Unit]
Description=LVS and VRRP High Availability Monitor
After=syslog.target network-online.target

[Service]
Type=forking
PIDFile=/var/run/keepalived.pid
KillMode=control-group
EnvironmentFile=-/etc/sysconfig/keepalived
ExecStart=/usr/sbin/keepalived $KEEPALIVED_OPTIONS
ExecReload=/bin/kill -HUP $MAINPID
ExecStop=/bin/kill -TERM $MAINPID

[Install]
WantedBy=multi-user.target
```

#### 问题
* 局域网无法Ping VIP
