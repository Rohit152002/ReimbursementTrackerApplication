using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReimbursementTrackingApplication.Context;
using ReimbursementTrackingApplication.Exceptions;
using ReimbursementTrackingApplication.Models;
using ReimbursementTrackingApplication.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReimbursementUnitProjectTest.Repositories
{
    internal class PaymentRepositoryTest
    {
        DbContextOptions options;
        ContextApp context;
        PaymentRepository repository;
        Mock<ILogger<PaymentRepository>> logger;

        [SetUp]
        public void Setup()
        {
            options = new DbContextOptionsBuilder<ContextApp>()
                   .UseInMemoryDatabase("database" + Guid.NewGuid())
                    .Options;
            context = new ContextApp(options);
            logger = new Mock<ILogger<PaymentRepository>>();
            repository = new PaymentRepository(context, logger.Object);
        }

        [Test]
        public async Task AddPaymentTest()
        {

            Payment payment = new Payment()
            {
                RequestId = 2,
                AmountPaid = 1000,
                PaymentMethod=PaymentMethod.Cash,
                PaymentStatus=PaymentStatus.Paid
            };
            var addedPayment = await repository.Add(payment);
            Assert.IsTrue(addedPayment.AmountPaid == payment.AmountPaid);

        }

        [Test]
        public async Task AddPaymentException()
        {

            Payment payment = new Payment()
            {
                
                AmountPaid = 1000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Paid
            };

            Assert.ThrowsAsync<CouldNotAddException>(async () => await repository.Add(payment));
        }

        [Test]
        public async Task DeletePayment()
        {
            Payment payment = new Payment()
            {
                RequestId = 2,
                AmountPaid = 1000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Paid
            };
            var addedPayment = await repository.Add(payment);
            var deletePayment = await repository.Delete(addedPayment.Id);
            Assert.AreEqual(payment.RequestId, deletePayment.RequestId);
        }

        [Test]
        public async Task DeletePaymentExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Delete(1));
        }

        [Test]
        public async Task GetAllPayment()
        {
            Payment payment = new Payment()
            {
                RequestId = 2,
                AmountPaid = 1000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Paid
            };
            await repository.Add(payment);
            var payments = await repository.GetAll();
            Assert.NotNull(payments);
        }

        [Test]
        public async Task GetAllPaymentException()
        {
            Assert.ThrowsAsync<CollectionEmptyException>(async () => await repository.GetAll());
        }

        [Test]
        public async Task UpdatePayment()
        {
            Payment payment = new Payment()
            {
                RequestId = 2,
                AmountPaid = 1000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Paid
            };
            await repository.Add(payment);

            Payment updatePayment = new Payment()
            {
                RequestId = 2,
                AmountPaid = 2000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Paid
            };
            var update = await repository.Update(1, updatePayment);
            Assert.AreEqual(update.AmountPaid, updatePayment.AmountPaid);
        }


        [Test]
        public async Task UpdatePaymentExceptionTest()
        {
            Assert.ThrowsAsync<Exception>(async () => await repository.Update(1, new Payment { PaymentMethod = PaymentMethod.MobilePayment }));

        }

        [Test]
        public async Task GetPayment()
        {
            Payment payment = new Payment()
            {
                RequestId = 2,
                AmountPaid = 1000,
                PaymentMethod = PaymentMethod.Cash,
                PaymentStatus = PaymentStatus.Paid
            };
            
            await repository.Add(payment);
            var getUser = await repository.Get(1);
            Assert.IsNotNull(getUser);
        }

        [Test]
        public async Task GetPaymentExceptions()
        {
            Assert.ThrowsAsync<NotFoundException>(async () => await repository.Get(1));
        }



    }
}
