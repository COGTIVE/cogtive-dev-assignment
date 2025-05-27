# Cores para melhor legibilidade
$Green = [System.ConsoleColor]::Green
$Yellow = [System.ConsoleColor]::Yellow

Write-Host "Parando os serviços da plataforma IoT Industrial..." -ForegroundColor $Yellow

# Parar e remover containers
docker-compose down

Write-Host "`nServiços parados com sucesso!" -ForegroundColor $Green 