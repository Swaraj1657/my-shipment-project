// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

/**
 * @title ShipmentTracker
 * @dev This contract stores and retrieves the history of cryptographic hashes
 * for shipment transactions, acting as an immutable ledger.
 */
contract ShipmentTracker {

    mapping(string => string[]) public shipmentHistory;


    event ShipmentUpdated(string indexed trackingId, string dataHash, uint timestamp);

    // --- Functions ---

    /**
     * @dev Adds a new transaction hash to a specific shipment's history.
     * This function should be called by your .NET backend application.
     * @param _trackingId The unique identifier for the shipment.
     * @param _dataHash The cryptographic hash (e.g., SHA-256) of the transaction data.
     */
    function addShipmentUpdate(string memory _trackingId, string memory _dataHash) public {
        // Adds the new hash to the end of the array for the given tracking ID.
        shipmentHistory[_trackingId].push(_dataHash);

        // Emits an event to log that this update occurred on the blockchain.
        // `block.timestamp` is a global variable that provides the timestamp of the current block.
        emit ShipmentUpdated(_trackingId, _dataHash, block.timestamp);
    }

    /**
     * @dev Retrieves the complete list of transaction hashes for a given shipment.
     * This is a 'view' function, meaning it only reads data and does not cost any gas to call.
     * @param _trackingId The unique identifier for the shipment to look up.
     * @return A memory array of strings containing all transaction hashes.
     */
    function getShipmentHistory(string memory _trackingId) public view returns (string[] memory) {
        // Returns the array of hashes stored for the specified tracking ID.
        return shipmentHistory[_trackingId];
    }
} 