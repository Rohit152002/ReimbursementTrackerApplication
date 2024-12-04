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
    maxAmount: +maxAmount,
    policyDescription,
  });
}

export function UpdatePolicy(id, policyName, maxAmount, policyDescription) {
  return axiosIntance.put(`/api/Policy/${id}`, {
    policyName,
    maxAmount: +maxAmount,
    policyDescription,
  });
}

export function DeletePolicy(id) {
  return axiosIntance.delete(`/api/Policy/${id}`);
}
