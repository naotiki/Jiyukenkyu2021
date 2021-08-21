# mlagents-learn.exe config/ppo/Pacman.yaml --run-id=test --time-scale=5 --quality-level=1
Set-Location C:\Projects\Unity\Ziyuukenkyu2021\ml-agents\
$input = Read-Host  "���_ID����͂��Ă�������" "Pacman-"
if($input -eq ""){
    $input="test"
}
$trainName="Pacman-"+$input
$p="C:\Projects\Unity\Ziyuukenkyu2021\ml-agents\results\"+$trainName
$isLoad=$false
if (Test-Path $p) {
    if ($input -eq "test") {
        Remove-Item $p -Recurse
        Write-Host "�㏑�����܂���"
    }else{
        $question = Read-Host  "���_ID���d�����Ă��܂� �O��̐��_�𑱍s���܂����H �㏑�����܂����H" "([c]ontinue/[o]verwrite/[N]othing)"
        if ($question -eq "C" -or $question -eq "c") {
            $isLoad=$true
            Write-Host "�O��̐��_�𑱍s���܂��B"
        }elseif ($question -eq "O" -or $question -eq "o") {
            Remove-Item $p
            Write-Host "�㏑�����܂����B"
        }
        else {
           
            Write-Host "�������L�����Z�����܂��B"
            Pause
            exit
    
        }
    }
    
}
./.venv/Scripts/activate
#���_
if ($isLoad) {
    Write-Host "���_�𑱍s���܂��B"
    #mlagents-learn.exe config/ppo/Pacman.yaml --run-id=$trainName --time-scale=5 --quality-level=1 --resume --no-graphics
    mlagents-learn.exe config/ppo/Pacman.yaml --env=Pacman --run-id=$trainName --num-envs=8 --resume --no-graphics
}else{
   # mlagents-learn.exe config/ppo/Pacman.yaml --run-id=$trainName --time-scale=5 --quality-level=1 --no-graphics
   mlagents-learn.exe config/ppo/Pacman.yaml --env=Pacman --run-id=$trainName --num-envs=8 --no-graphics
}

if($input -ne "test"){
    $question = Read-Host  "�ۑ����܂����H" "([Y]es/[n]o)"
    if ($question -eq "N" -or $question -eq "n") {
        Remove-Item $p
    }
    else {
    
        Write-Host "�ۑ����܂���"
        Write-Host $p
    }
}

Pause