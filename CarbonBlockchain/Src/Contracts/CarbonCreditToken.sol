// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "../../node_modules/@openzeppelin/contracts/token/ERC1155/ERC1155.sol";
import "../../node_modules/@openzeppelin/contracts/access/Ownable.sol";

struct CarbonProject {
    string projectName;
    string country;
}

struct CarbonCredit {
    bytes16 creditCode;
    uint256 vintageYear;
    uint256 tonCO2Quantity;
    bool isRetired;
    uint256 retireAt;
    uint256 createdAt;
    uint256 modifiedAt;
    uint256 pricePerTon;
    string ownerName;
    CarbonProject project;
}

abstract contract CarbonCreditToken is ERC1155, Ownable {
    constructor(string memory uri) ERC1155(uri) {}
}
