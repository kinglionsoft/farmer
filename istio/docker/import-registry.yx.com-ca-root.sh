mkdir /usr/share/ca-certificates/yx.com
wget http://share.yx.com/registry.yx.com_ca_root.crt -O /usr/share/ca-certificates/yx.com/ca.crt
update-ca-certificates