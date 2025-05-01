// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

import "../../node_modules/@openzeppelin/contracts/token/ERC1155/ERC1155.sol";
import "../../node_modules/@openzeppelin/contracts/access/Ownable.sol";

abstract contract CarbonCreditToken is ERC1155, Ownable {
    constructor(string memory uri) ERC1155(uri) {}
}
