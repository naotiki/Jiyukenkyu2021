# mlagents-learn.exe config/ppo/Pacman.yaml --run-id=test --time-scale=5 --quality-level=1
Set-Location C:\Projects\Unity\Ziyuukenkyu2021\ml-agents\
$input = Read-Host  "推論IDを入力してください" "Pacman-"
if($input -eq ""){
    $input="test"
}
$trainName="Pacman-"+$input
$p="C:\Projects\Unity\Ziyuukenkyu2021\ml-agents\results\"+$trainName
$isLoad=$false
if (Test-Path $p) {
    if ($input -eq "test") {
        Remove-Item $p -Recurse
        Write-Host "上書きしました"
    }else{
        $question = Read-Host  "推論IDが重複しています 前回の推論を続行しますか？ 上書きしますか？" "([c]ontinue/[o]verwrite/[N]othing)"
        if ($question -eq "C" -or $question -eq "c") {
            $isLoad=$true
            Write-Host "前回の推論を続行します。"
        }elseif ($question -eq "O" -or $question -eq "o") {
            Remove-Item $p
            Write-Host "上書きしました。"
        }
        else {
           
            Write-Host "処理をキャンセルします。"
            Pause
            exit
    
        }
    }
    
}
./.venv/Scripts/activate
#推論
if ($isLoad) {
    Write-Host "推論を続行します。"
    #mlagents-learn.exe config/ppo/Pacman.yaml --run-id=$trainName --time-scale=5 --quality-level=1 --resume --no-graphics
    mlagents-learn.exe config/ppo/Pacman.yaml --env=Pacman --run-id=$trainName --num-envs=8 --resume --no-graphics
}else{
   # mlagents-learn.exe config/ppo/Pacman.yaml --run-id=$trainName --time-scale=5 --quality-level=1 --no-graphics
   mlagents-learn.exe config/ppo/Pacman.yaml --env=Pacman --run-id=$trainName --num-envs=8 --no-graphics
}

if($input -ne "test"){
    $question = Read-Host  "保存しますか？" "([Y]es/[n]o)"
    if ($question -eq "N" -or $question -eq "n") {
        Remove-Item $p
    }
    else {
    
        Write-Host "保存しました"
        Write-Host $p
    }
}

Pause