import axiosIntance from "./Interceptor";

export function getAllPolicy(pageNumber, pageSize) {
  return axiosIntance.get("/api/Policy", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function AddPolicy(policyName, maxAmount, policyDescription) {
  return axiosIntance.post("/api/Policy", {
    policyName,
    maxAmount,
    policyDescription,
  });
}
