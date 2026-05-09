$pages = @("MedicalDevices","Calibration","Documents","Risks","WorkOrders","Maintenance","ElectricalSafety","SpareParts","Reports","Users","Dashboard","Index")
foreach ($p in $pages) {
    try {
        $r = Invoke-WebRequest -Uri "http://localhost:5232/$p" -UseBasicParsing -TimeoutSec 60 -ErrorAction Stop
        Write-Host "$p`: $($r.StatusCode)"
    } catch [System.Net.WebException] {
        $er = $_.Exception.Response
        if ($er) {
            $sr = New-Object System.IO.StreamReader($er.GetResponseStream())
            $c = $sr.ReadToEnd()
            if ($c -like '*Scripts*already*') {
                Write-Host "$p`: *** DUPLICATE SCRIPTS ERROR ***"
            } else {
                $short = $c.Substring(0, [Math]::Min(300, $c.Length))
                Write-Host "$p`: $short"
            }
        }
    }
}
