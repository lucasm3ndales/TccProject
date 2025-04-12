namespace CarbonBlockchain.Services.CarbonCreditHandler.Enums;

public enum CarbonProjectType
{
    REFORESTATION = 1,                 // Reflorestamento ou plantio de novas árvores.
    AFFORESTATION = 2,                 // Florestamento (criação de florestas em áreas sem vegetação).
    RENEWABLE_ENERGY = 3,              // Energia renovável (solar, eólica, etc.).
    ENERGY_EFFICIENCY = 4,             // Projetos para redução do consumo energético.
    WASTE_MANAGEMENT = 5,              // Gestão de resíduos e captura de metano.
    AGRICULTURE = 6,                   // Práticas agrícolas sustentáveis.
    FOREST_CONSERVATION = 7,           // Conservação de florestas existentes (REDD+).
    SOIL_CARBON_SEQUESTRATION = 8,     // Sequestro de carbono no solo.
    WETLAND_RESTORATION = 9,           // Restauração de áreas úmidas e manguezais.
    BLUE_CARBON = 10,                  // Projetos relacionados a ecossistemas marinhos.
    INDUSTRIAL_PROCESSES = 11,         // Redução de emissões em processos industriais.
    CARBON_CAPTURE_AND_STORAGE = 12,   // Captura e armazenamento de carbono (CCS).
    OTHER = 99                         // Outros tipos de projetos personalizados.
}
