namespace CarbonCertifier.Entities.CarbonProject.Enums;

public enum CarbonProjectStatus
{
    PLANNED, // Projeto planejado, mas não iniciado.
    ACTIVE, // Projeto em execução e monitoramento ativo.
    CERTIFIED, // Projeto verificado e aprovado por certificadores.
    SUSPENDED, // Projeto suspenso temporariamente.
    COMPLETED, // Projeto concluído e sem novas emissões de crédito.
    EXPIRED, // Projeto expirado (não mais certificado ou válido).
    CANCELLED
}