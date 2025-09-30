import assert from "node:assert/strict";
import { describe, it } from "node:test";

import { network } from "hardhat";

describe("ShipmentTracker", async function () {
  const { viem } = await network.connect();
  const publicClient = await viem.getPublicClient();

  it("Should add a shipment update and emit the ShipmentUpdated event", async function () {
    const shipmentTracker = await viem.deployContract("ShipmentTracker");
    const trackingId = "TRACK123";
    const dataHash = "0x123456789abcdef";

    const tx = await shipmentTracker.write.addShipmentUpdate([trackingId, dataHash]);
    

    const receipt = await publicClient.getTransactionReceipt({
      hash: tx,
    });
    

    const logs = await publicClient.getContractEvents({
      address: shipmentTracker.address,
      abi: shipmentTracker.abi,
      eventName: "ShipmentUpdated",
      fromBlock: receipt.blockNumber,
      toBlock: receipt.blockNumber,
    });
    
    assert.equal(logs.length, 1);
    // For indexed string parameters, we don't check the exact value as it's stored as a hash
    assert.equal(logs[0]?.args?.dataHash, dataHash);
    assert(logs[0]?.args?.timestamp !== undefined && logs[0].args.timestamp > 0n, "Timestamp should be greater than 0");
  });

  it("Should retrieve shipment history correctly", async function () {
    const shipmentTracker = await viem.deployContract("ShipmentTracker");
    const trackingId = "TRACK456";
    const dataHash1 = "0xabc123";
    const dataHash2 = "0xdef456";

    // Add two updates to the shipment
    await shipmentTracker.write.addShipmentUpdate([trackingId, dataHash1]);
    await shipmentTracker.write.addShipmentUpdate([trackingId, dataHash2]);

    // Retrieve the history
    const history = await shipmentTracker.read.getShipmentHistory([trackingId]);

    assert.equal(history.length, 2);
    assert.equal(history[0], dataHash1);
    assert.equal(history[1], dataHash2);
  });
});