namespace CarbonCertifier.Entities.CarbonCredit.Enums;

public enum CarbonCreditStatus
{
    PENDING_ISSUANCE, // Crédito aprovado, mas não emitido
    ISSUED, // Crédito emitido e registrado
    AVAILABLE, // Disponível para negociação ou uso
    TRANSFERRED, // Transferido para outro dono
    RETIRED, // Usado para compensação (não reutilizável)
    CANCELLED, // Cancelado antes do uso
}