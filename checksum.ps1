function Get-Hashes($filePath) {
    $md5 = [System.Security.Cryptography.MD5]::Create()
    $sha1 = [System.Security.Cryptography.SHA1]::Create()
    $sha256 = [System.Security.Cryptography.SHA256]::Create()

    $hashes = @{
        MD5 = ''
        SHA1 = ''
        SHA256 = ''
    }

    $fileStream = $null

    try {
        $fileStream = [System.IO.File]::OpenRead($filePath)

        $hashes.MD5 = [BitConverter]::ToString($md5.ComputeHash($fileStream)) -replace '-', ''
        $fileStream.Position = 0
        $hashes.SHA1 = [BitConverter]::ToString($sha1.ComputeHash($fileStream)) -replace '-', ''
        $fileStream.Position = 0
        $hashes.SHA256 = [BitConverter]::ToString($sha256.ComputeHash($fileStream)) -replace '-', ''
    } catch {
        Write-Host "Error processing file: $filePath. $_"
    } finally {
        if ($fileStream) { $fileStream.Close() }
    }

    return $hashes
}

$directoryPath = "C:\Program Files\Sefinek\Genshin-Stella-Mod"
$desktopPath = [environment]::GetFolderPath('Desktop')
$fileList = Get-ChildItem -Path $directoryPath -File -Recurse

$hashData = @()

foreach ($file in $fileList) {
    Write-Host "Processing file: $($file.FullName)"
    $relativePath = $file.FullName.Substring($directoryPath.Length).TrimStart("\")
    $hashes = Get-Hashes $file.FullName
    $hashData += [PSCustomObject]@{
        FilePath = $relativePath
        MD5Hash  = $hashes.MD5
        SHA1Hash = $hashes.SHA1
        SHA256Hash = $hashes.SHA256
    }
}

$hashData | ConvertTo-Json -Depth 10 | ForEach-Object { $_ -replace '":\s{2,}', '": ' } | Set-Content -Path "$desktopPath\file_hashes.json"
Write-Host "Hash calculation completed. Results saved to $desktopPath\file_hashes.json"
