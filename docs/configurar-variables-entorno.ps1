<#
    ============================================================================
    Configura las variables de entorno necesarias para WsUtaDinardap.Api y para
    la DLL legacy DatosDINARDAP.dll en un servidor Windows (IIS).

    Ejecutar como Administrador (PowerShell) EN EL SERVIDOR donde corresponda
    cada bloque - no en tu maquina local, y nunca commitear este archivo con
    los valores reales rellenados.

    Las variables quedan a nivel de MAQUINA (persisten tras reiniciar), asi
    que despues de correr esto hay que RECICLAR el Application Pool de IIS
    (o reiniciar el servicio/proceso que corresponda) para que el proceso
    las vea - un proceso ya corriendo no relee variables de entorno nuevas.
    ============================================================================
#>

# ----------------------------------------------------------------------------
# BLOQUE 1 - Servidor donde se publica WsUtaDinardap.Api
# ----------------------------------------------------------------------------
# Reemplaza los valores entre <...> por los reales antes de ejecutar.
# Estas 2 son las credenciales reales de DINARDAP (las que hoy estan en
# appSettings.Production.json como "VARIABLE_DE_ENTORNO").

[System.Environment]::SetEnvironmentVariable('Dinardap__Username', '<usuario_real_dinardap>', 'Machine')
[System.Environment]::SetEnvironmentVariable('Dinardap__Password', '<password_real_dinardap>', 'Machine')

# Opcional - solo si quieres sobreescribir sin tocar appsettings.Production.json
# (si no la defines, se usa el valor que ya tiene ese archivo: AuthService:Url)
# [System.Environment]::SetEnvironmentVariable('AuthService__Url', 'https://serviciospruebas.uta.edu.ec/WsSeguUta', 'Machine')

Write-Host "Bloque 1 (WsUtaDinardap.Api) aplicado. Recicla el Application Pool de IIS para que tome efecto." -ForegroundColor Green

# ----------------------------------------------------------------------------
# BLOQUE 2 - Servidor(es) donde corre cada sistema consumidor de
#            DatosDINARDAP.dll (legacy)
# ----------------------------------------------------------------------------
# Correr este bloque en CADA host que tenga la dll referenciada, no solo una vez.
# El valor real de este secreto NO va en este script ni en ningun archivo del
# repo - se te entrego aparte, en el chat, fuera de git.

# [System.Environment]::SetEnvironmentVariable('WSUTADINARDAP_CLIENT_SECRET', '<pegar_aqui_el_secreto_entregado>', 'Machine')

Write-Host "Bloque 2 (DatosDINARDAP.dll legacy) queda comentado a proposito - descomentar linea por linea, pegar el secreto real, y borrar el valor del historial de PowerShell despues (Clear-History)." -ForegroundColor Yellow

# ----------------------------------------------------------------------------
# Verificacion (correr en una consola NUEVA, ya que las variables de Machine
# no aparecen en la sesion actual hasta abrir una consola nueva o reiniciar
# el proceso que las va a consumir)
# ----------------------------------------------------------------------------
# Get-ChildItem Env: | Where-Object { $_.Name -like 'Dinardap*' -or $_.Name -like 'AuthService*' -or $_.Name -eq 'WSUTADINARDAP_CLIENT_SECRET' }

<#
    ----------------------------------------------------------------------------
    ALTERNATIVA para WsUtaDinardap.Api si prefieres no tocar variables de
    entorno de toda la maquina: IIS tambien permite definirlas por sitio, en
    el web.config generado por "dotnet publish" (no se versiona en git, se
    edita directo en el servidor despues de publicar):

    <configuration>
      <location path="." inheritInChildApplications="false">
        <system.webServer>
          <aspNetCore ...>
            <environmentVariables>
              <environmentVariable name="Dinardap__Username" value="<usuario_real_dinardap>" />
              <environmentVariable name="Dinardap__Password" value="<password_real_dinardap>" />
            </environmentVariables>
          </aspNetCore>
        </system.webServer>
      </location>
    </configuration>

    Con esta alternativa el reciclado del Application Pool tambien es necesario
    despues de guardar el web.config.
    ----------------------------------------------------------------------------
#>
