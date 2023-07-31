mkdir -p /opt/wg/wg-worker
cp -u WG.Worker /opt/wg/wg-worker/wg-worker
cp  -n appsettings.json /opt/wg/wg-worker
chmod +x /opt/wg/wg-worker/wg-worker
cp wg-worker.service /etc/systemd/system/
systemctl daemon-reload
systemctl start wg-worker.service
systemctl status wg-worker.service
