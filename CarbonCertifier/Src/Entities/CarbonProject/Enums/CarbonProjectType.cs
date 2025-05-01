namespace CarbonCertifier.Entities.CarbonProject.Enums;

public enum CarbonProjectType
{
    REFORESTATION, // Plantio de árvores em áreas degradadas.
    FOREST_CONSERVATION, // REDD+ — Evita o desmatamento de florestas existentes.
    RENEWABLE_ENERGY, // Energia solar, eólica, hidrelétrica de pequeno porte.
    ENERGY_EFFICIENCY, // Redução de consumo ou perdas energéticas.
    WASTE_MANAGEMENT, // Tratamento de resíduos, captura de metano.
    CARBON_CAPTURE_AND_STORAGE, // CCS — Tecnologias industriais de captura de carbono.
    AGRICULTURE, // Manejo de solo e práticas agrícolas de baixo carbono.
    OTHER // Reservado para projetos que não se encaixam nos anteriores.
}