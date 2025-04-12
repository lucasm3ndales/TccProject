namespace CarbonBlockchain.Services.CarbonCreditHandler.Enums;
public enum CarbonProjectStatus
{
    PLANNED = 1,            // Projeto planejado, mas não iniciado.
    ACTIVE = 2,             // Projeto em execução e monitoramento ativo.
    CERTIFIED = 3,           // Projeto verificado e aprovado por certificadores.
    SUSPENDED = 4,          // Projeto suspenso temporariamente.
    COMPLETED = 5,          // Projeto concluído e sem novas emissões de crédito.
    EXPIRED = 6,            // Projeto expirado (não mais certificado ou válido).
    CANCELED = 7            // Projeto cancelado antes da certificação.
}