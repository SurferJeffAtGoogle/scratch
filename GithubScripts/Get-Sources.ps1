# Meant to run across a directory containing all repos in googleapis.

$metadataFilePaths = gci -Recurse -Filter synth.metadata -Exclude .git
$sources = foreach ($path in $metadataFilePaths) {
    $metadata = ConvertFrom-Json (Get-Content -Raw $path)
    $metadata.sources
}
$gitSources = $sources | Where-Object {$_.git} | Select -ExpandProperty git 
$generatorSources = $sources | Where-Object {$_.generator} | Select -ExpandProperty generator
$templateSources = $sources | Where-Object {$_.template} | Select -ExpandProperty template

$gitSources | Sort-Object | ConvertTo-Csv | Out-File git-sources.csv
$generatorSources | Sort-Object | ConvertTo-Csv | Out-File generator-sources.csv
$templateSources | Sort-Object | ConvertTo-Csv | Out-File template-sources.csv



