Set-Location C:\Projects\Unity\Ziyuukenkyu2021\ml-agents\
./.venv/Scripts/activate
start 'http://localhost:6006'
tensorboard --logdir results --host 0.0.0.0 --port 6006
