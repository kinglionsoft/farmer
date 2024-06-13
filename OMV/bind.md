# DNS

## 配置

### named.conf

```

include "/etc/bind/named.conf.options";
include "/etc/bind/named.conf.local";
// include "/etc/bind/named.conf.default-zones"; // Move to views


```

### named.conf.options

```
// ...

view "internal" {  
    match-clients { 10.147.19.0/24; }; // 匹配内部网络的IP地址  
  
    zone "y.home" IN {  
        type master;  
        file "/etc/bind/db.y.home.internal"; // 内部网络使用的区域文件  
    };  
  
    include "/etc/bind/named.conf.default-zones";
};  
  
view "external" {  
    match-clients { any; }; // 匹配所有其他请求（除了上面定义的内部网络）  
  
    zone "y.home" IN {  
        type master;  
        file "/etc/bind/db.y.home.external"; // 外部网络使用的区域文件  
    };  
  
    include "/etc/bind/named.conf.default-zones";
};

```

### db.y.home.internal

```
$TTL	86400
@	IN	SOA	localhost. root.localhost. (
			      1		; Serial
			 604800		; Refresh
			  86400		; Retry
			2419200		; Expire
			  86400 )	; Negative Cache TTL
;
@	IN	NS	localhost.
pan	    IN	A	10.147.19.30
admin	IN	A	10.147.19.30
```

### db.y.home.external

```
$TTL	86400
@	IN	SOA	localhost. root.localhost. (
			      1		; Serial
			 604800		; Refresh
			  86400		; Retry
			2419200		; Expire
			  86400 )	; Negative Cache TTL
;
@	IN	NS	localhost.
pan	    IN	A	192.168.0.30
admin	IN	A	192.168.0.30
```