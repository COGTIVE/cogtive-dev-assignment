# Cores para melhor legibilidade
$Green = [System.ConsoleColor]::Green
$Yellow = [System.ConsoleColor]::Yellow
$Red = [System.ConsoleColor]::Red

Write-Host "Iniciando a plataforma IoT Industrial..." -ForegroundColor $Yellow

# Verificar se o Docker está instalado
try {
    docker --version | Out-Null
}
catch {
    Write-Host "Docker não está instalado. Por favor, instale o Docker Desktop primeiro." -ForegroundColor $Red
    exit 1
}

# Verificar se o Docker está rodando
try {
    docker info | Out-Null
}
catch {
    Write-Host "Docker não está rodando. Por favor, inicie o Docker Desktop." -ForegroundColor $Red
    exit 1
}

# Verificar se os Dockerfiles existem
$requiredFiles = @(
    "backend/Dockerfile",
    "web/Dockerfile",
    "iot-simulator/Dockerfile"
)

foreach ($file in $requiredFiles) {
    if (-not (Test-Path $file)) {
        Write-Host "Erro: Arquivo $file não encontrado" -ForegroundColor $Red
        exit 1
    }
}

# Parar e remover containers existentes
Write-Host "Limpando containers existentes..." -ForegroundColor $Yellow
docker-compose down

# Construir e iniciar os containers
Write-Host "Construindo e iniciando os containers..." -ForegroundColor $Yellow
docker-compose up --build -d

# Aguardar um pouco para os containers iniciarem
Start-Sleep -Seconds 10

# Verificar se os containers estão rodando
$containers = docker-compose ps --services
$allRunning = $true

foreach ($container in $containers) {
    $status = docker-compose ps -q $container
    if (-not $status) {
        $allRunning = $false
        Write-Host "Erro: Container $container não está rodando" -ForegroundColor $Red
        Write-Host "Logs do container:" -ForegroundColor $Yellow
        docker-compose logs $container
    }
}

if ($allRunning) {
    Write-Host "`nTodos os serviços estão rodando:" -ForegroundColor $Green
    Write-Host "- Backend API: http://localhost:5000" -ForegroundColor $Green
    Write-Host "- Frontend Web: http://localhost:3000" -ForegroundColor $Green
    Write-Host "- Simulador IoT: http://localhost:4000" -ForegroundColor $Green
    Write-Host "`nDocumentação da API: http://localhost:5000/swagger" -ForegroundColor $Green
} else {
    Write-Host "`nAlguns serviços não iniciaram corretamente. Verifique os logs com 'docker-compose logs'" -ForegroundColor $Red
} 