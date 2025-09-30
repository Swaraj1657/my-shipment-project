import { buildModule } from "@nomicfoundation/hardhat-ignition/modules";

export default buildModule("ShipmentTrackerModule", (m) => {
  const shipmentTracker = m.contract("ShipmentTracker");
  
  return { shipmentTracker };
});