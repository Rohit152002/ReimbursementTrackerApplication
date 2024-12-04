import axiosIntance from "./Interceptor";

export function getPaymentByRequestId(id) {
  return axiosIntance.get(`api/Payment/${id}`);
}

export function postPayment(id, paymentMethod) {
  return axiosIntance.put("api/Payment", {
    id: +id,
    paymentMethod: +paymentMethod,
  });
}

export function getPaymentByUser(id, pageNumber, pageSize) {
  return axiosIntance.get(`/api/Payment/user/${id}`, {
    params: {
      pageNumber,
      pageSize,
    },
  });
}
export function getAllPayment(pageNumber, pageSize) {
  return axiosIntance.get("api/Payment/all", {
    params: {
      pageNumber,
      pageSize,
    },
  });
}
