import axiosIntance from "./Interceptor";

export function getAllBanksDetails(pageNumber, pageSize) {
  return axiosIntance.get("/api/Bank/all", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}

export function getBankByUser(userId) {
  return axiosIntance.get(`/api/Bank/user/${userId}`);
}

export function addBank(userId, accNo, branchName, ifscCode, branchAddress) {
  return axiosIntance.post("/api/Bank", {
    userId,
    accNo,
    branchName,
    ifscCode,
    branchAddress,
  });
}
