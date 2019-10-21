using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using PALMS.Data.Objects.ClientModel;
using PALMS.Data.Objects.InvoiceModel;
using PALMS.Data.Objects.NoteModel;
using PALMS.Invoices.ViewModel.EntityViewModel;
using PALMS.ViewModels.Common.Enumerations;

namespace PALMS.Invoices.ViewModel.Service
{
    public class PriceCounting : ViewModelBase, IClear
    {
        private InvoiceSumViewModel _invoiceSum;
        private List<InvoiceItems> _invoiceDepartments;
        private List<InvoiceItems> _previewChargeItems;

        public List<InvoiceItems> PreviewChargeItems
        {
            get => _previewChargeItems;
            set => Set(ref _previewChargeItems, value);
        }
        public List<InvoiceItems> InvoiceDepartments
        {
            get => _invoiceDepartments;
            set => Set(ref _invoiceDepartments, value);
        }
        public InvoiceSumViewModel InvoiceSum
        {
            get => _invoiceSum;
            set => Set(ref _invoiceSum, value);
        }

        public InvoiceSumViewModel GetInvoiceSum(InvoiceSumViewModel invoiceSum, List<TaxAndFees> taxAndFees, List<ExtraCharge> extraCharges)
        {
            InvoiceSum = invoiceSum;
            InvoiceDepartments = CalcDepartment(InvoiceSum.Departments);
            if((taxAndFees == null || taxAndFees.Count == 0) && (extraCharges == null || extraCharges.Count == 0))
            {
                GetInvoicePreViews();
            }else
            {
                GetInvoicePreViews(taxAndFees, extraCharges);
            }

            InvoiceSum.Amount = 0;
            foreach (var item in PreviewChargeItems)
            {
                InvoiceSum.Amount += item.Amount;
            }

            InvoiceSum.VatAmount = InvoiceSum.Amount * InvoiceSum.Vat;

            return InvoiceSum;
        }

        public List<InvoiceItems> CalculateNoteRows(List<NoteRow> rows)
        {
            var invNoteRows = new List<InvoiceItems>();

            foreach (var row in rows)
            {
                var invNoteRow = new InvoiceItems(row)
                {
                    Amount = row.ClientReceivedQty * row.Price,
                    QtyCollected = row.PrimeCollectionQty,
                    QtyDelivered = row.PrimeDeliveryQty,
                    QtyClientReceived = row.ClientReceivedQty,
                };

                invNoteRows.Add(invNoteRow);
            }

            return invNoteRows;
        }

        public List<InvoiceItems> CalculateNoteHeader(List<NoteHeader> noteHeaders)
        {
            var invNoteHeaders = new List<InvoiceItems>();

            if (noteHeaders == null || noteHeaders.Count == 0) return invNoteHeaders;

            foreach (var noteHeader in noteHeaders)
            {
                var invNoteHeader = new InvoiceItems(noteHeader)
                {
                    Amount = noteHeader.DeliveryWeight * noteHeader.WeightPrice,
                    Name = noteHeader.Name,
                    WeightCollected = noteHeader.CollectionWeight,
                    WeightDelivered = noteHeader.DeliveryWeight,
                };

                var noteRows = InvoiceSum.NoteRows.Where(x => x.NoteHeaderId == noteHeader.Id).ToList();
                var invNoteRows = CalculateNoteRows(noteRows);

                foreach (var noteRow in invNoteRows)
                {
                    invNoteHeader.QtyCollected += noteRow.QtyCollected;
                    invNoteHeader.QtyDelivered += noteRow.QtyDelivered;
                    invNoteHeader.QtyClientReceived += noteRow.QtyClientReceived;
                    invNoteHeader.Amount += noteRow.Amount;
                }

                switch (noteHeader.DeliveryTypeId)
                {
                    case (int) DeliveryTypeEnum.ReWash:
                        invNoteHeader.Amount = 0;
                        break;
                    case (int) DeliveryTypeEnum.Express:
                        invNoteHeader.Amount += invNoteHeader.Amount * noteHeader.ExpressCharge;
                        break;
                }

                invNoteHeaders.Add(invNoteHeader);
            }

            return invNoteHeaders;
        }

        public List<InvoiceItems> CalcDepartment(List<Department> departments)
        {
            var invDepartments = new List<InvoiceItems>();

            // Get All department values
            foreach (var department in departments)
            {
                var notes = InvoiceSum.NoteHeaders.Where(x => x.DepartmentId == department.Id).ToList();
                if (notes.Count == 0)
                {
                    var dep = new InvoiceItems(department)
                    {
                        Name = department.Name,
                    };
                    invDepartments.Add(dep);

                    continue;
                }

                var invNoteHeaders = CalculateNoteHeader(notes);

                var invDepartment = new InvoiceItems(department)
                {
                    Name = department.Name,
                };

                foreach (var invNote in invNoteHeaders)
                {
                    invDepartment.Amount += invNote.Amount;
                    invDepartment.QtyCollected += invNote.QtyCollected;
                    invDepartment.QtyDelivered += invNote.QtyDelivered;
                    invDepartment.QtyClientReceived += invNote.QtyClientReceived;
                    invDepartment.WeightDelivered += invNote.WeightDelivered;
                    invDepartment.WeightCollected += invNote.WeightCollected;
                }

                invDepartments.Add(invDepartment);
            }

            // Add child department values to parent department
            foreach (var subDepartment in invDepartments.Where(x=> ((Department) x.OriginalObject).ParentId != null))
            {
                var parentDep = invDepartments.FirstOrDefault(x=> x.Id == ((Department) subDepartment.OriginalObject).ParentId);

                if (parentDep == null) continue;
                parentDep.QtyCollected += subDepartment.QtyCollected;
                parentDep.QtyDelivered += subDepartment.QtyDelivered;
                parentDep.QtyClientReceived += subDepartment.QtyClientReceived;
                parentDep.WeightCollected += subDepartment.WeightCollected;
                parentDep.WeightDelivered += subDepartment.WeightDelivered;
                parentDep.Amount += subDepartment.Amount;
            }

            //Calculate department contracts/discounts
            invDepartments = CalcDepartmentContract(invDepartments);

            return invDepartments;
        }

        public List<InvoiceItems> CalcDepartmentContract(List<InvoiceItems> departments)
        {
            var invDepartments = departments;

            foreach (var department in invDepartments)
            {
                if (((Department)department.OriginalObject).AllFree)
                {
                    department.DiscountAmount = department.Amount;
                    continue;
                }

                department.DiscountAmount = CalcDepartmentContract(department);

                department.Amount = Math.Round((Double) department.Amount, 2);
                department.DiscountAmount = Math.Round((Double) department.DiscountAmount, 2);
            }

            foreach (var department in invDepartments)
            {
                if (((Department) department.OriginalObject).ParentId != null) continue;

                foreach (var subDepartment in invDepartments.Where(x =>
                    ((Department) x.OriginalObject).ParentId == department.Id))
                {
                    department.DiscountAmount += subDepartment.DiscountAmount;
                }

                if (((Department) department.OriginalObject).AllFree)
                {
                    department.DiscountAmount = department.Amount;
                }
            }

            return invDepartments;
        }

        public double CalcDepartmentContract(InvoiceItems department)
        {
            var rows = new List<NoteRow>();

            var contracts =
                InvoiceSum.DepartmentContracts.Where(x =>
                    x.DepartmentId == ((Department)department.OriginalObject).Id);

            var notes = InvoiceSum.NoteHeaders.Where(x => x.DepartmentId == ((Department) department.OriginalObject).Id).ToList();

            foreach (var note in notes)
            {
                rows.AddRange(InvoiceSum.NoteRows.Where(x => x.NoteHeaderId == note.Id));
            }

            if (((Department)department.OriginalObject).ParentId == null || ((Department)department.OriginalObject).ParentId == 0)
            {
                foreach (var inDepartment in InvoiceSum.Departments.Where(x => x.ParentId == department.OriginalObject.Id))
                {
                    notes.AddRange(InvoiceSum.NoteHeaders.Where(x => x.DepartmentId == inDepartment.Id));
                }
            }

            if (!contracts.Any() || rows.Count == 0)
            {
                return 0;
            }

            var departmentDiscount = 0.0;
            var sortedRows = NoteRowSumDuplicatesByFamilyId(rows);
            
            foreach (var contract in contracts.OrderBy(x => x.OrderNumber))
            {
                var quantity = 0;
                var discount = 0.00;

                if (!contract.FamilyLinenId.HasValue)
                {
                    if (contract.Quantity != null)
                    {
                        quantity = (int)contract.Quantity;
                    }

                    if (contract.Quantity > department.QtyClientReceived ||
                        contract.Quantity == 0)
                    {
                        quantity = (int)department.QtyClientReceived;
                    }

                    var average = department.Amount / department.QtyClientReceived;
                    if (!(average >= 0.00))
                    {
                        average = 0.00;
                    }

                    discount += contract.Percentage * average * quantity;
                }
                else
                {
                    var noteRow = sortedRows?.FirstOrDefault(x => ((NoteRow) x.OriginalObject).LinenList.MasterLinen.FamilyLinenId == contract.FamilyLinenId);
                    if (noteRow == null) return 0;

                    if (contract.Quantity != null)
                    {
                        quantity = (int)contract.Quantity;
                    }

                    if (contract.Quantity > noteRow.QtyClientReceived || contract.Quantity == 0)
                    {
                        quantity = (int)noteRow.QtyClientReceived;
                    }

                    var average = noteRow.Amount / noteRow.QtyClientReceived;
                    discount = contract.Percentage * average * quantity;
                }

                departmentDiscount += discount;
            }

            return departmentDiscount;
        }

        public List<InvoiceItems> NoteRowSumDuplicatesByFamilyId(List<NoteRow> noteRows)
        {
            var sortedNoteRows = new List<InvoiceItems>();

            foreach (var notRow in noteRows)
            {
                var availableRow = sortedNoteRows.Find(x =>
                    ((NoteRow) x.OriginalObject).LinenList.MasterLinen.FamilyLinenId ==
                    notRow.LinenList.MasterLinen.FamilyLinenId);

                if (availableRow != null)
                {
                    availableRow.Amount += notRow.Price * notRow.ClientReceivedQty;
                    availableRow.QtyClientReceived += notRow.ClientReceivedQty;
                    availableRow.QtyCollected += notRow.PrimeCollectionQty;
                    availableRow.QtyDelivered += notRow.PrimeDeliveryQty;
                }
                else
                {
                    sortedNoteRows.Add(new InvoiceItems(notRow)
                    {
                        Amount = notRow.Price * notRow.ClientReceivedQty,
                        QtyClientReceived = notRow.ClientReceivedQty,
                        QtyDelivered = notRow.PrimeDeliveryQty,
                        QtyCollected = notRow.PrimeCollectionQty,
                    });
                }
            }
            return sortedNoteRows;
        }

        public List<InvoiceItems> GetNotesSummary()
        {
            var notesSummary = new List<InvoiceItems>();

            notesSummary.AddRange(InvoiceDepartments);

            foreach (var department in InvoiceDepartments)
            {
                department.ParentId = ((Department)department.OriginalObject).ParentId;

                var invNotes = CalculateNoteHeader(InvoiceSum.NoteHeaders.Where(x => x.DepartmentId == department.OriginalObject.Id)
                    .ToList());

                invNotes.ForEach(x => x.ParentId = department.Id);
                notesSummary.AddRange(invNotes);
            }

            return notesSummary.OrderBy(x => x.Name).ToList();
        }

        public List<InvoiceItems> GetInvoicePreViews()
        {
            PreviewChargeItems = new List<InvoiceItems>();

            foreach (var department in InvoiceDepartments.Where(x => ((Department)x.OriginalObject).ParentId == null))
            {
                var newItem = new InvoiceItems(department.OriginalObject)
                {
                    Name = department.Name,
                    PId = GetSubTotalId(),
                    Amount = department.Amount - department.DiscountAmount,
                    VatAmount = (department.Amount - department.DiscountAmount) * InvoiceSum.Vat,
                    QtyClientReceived = department.QtyClientReceived,
                    QtyDelivered = department.QtyDelivered,
                    QtyCollected = department.QtyCollected,
                };

                PreviewChargeItems.Add(newItem);
            }

            return PreviewChargeItems;
        }

        public List<InvoiceItems> GetInvoicePreViews(List<TaxAndFees> taxAndFees, List<ExtraCharge> extraCharges)
        {
            PreviewChargeItems = GetInvoicePreViews();

            if (taxAndFees != null && taxAndFees.Any())
            {
               CalcTaxAndCharges(taxAndFees);
            }

            if (extraCharges != null && extraCharges.Any())
            {
                CalcExtraCharges(extraCharges);
            }

            return PreviewChargeItems;
        }

        public void CalcTaxAndCharges(List<TaxAndFees> taxAndFees)
        {
            if (taxAndFees == null || taxAndFees.Count == 0)
                return;

            foreach (var taxAndFee in taxAndFees.OrderBy(x => x.Priority))
            {
                var number = taxAndFee.Number;
                var amount = new double();

                var totalAmount = new double();
                PreviewChargeItems.ForEach(x=> totalAmount+= x.Amount);

                switch (taxAndFee.UnitId)
                {
                    case (int)FeeUnitEnum.AED:
                        amount = number;
                        break;
                    case (int)FeeUnitEnum.Percentage:
                        amount = totalAmount * (number / 100);
                        number = 0;
                        break;
                }

                PreviewChargeItems.Add(new InvoiceItems(taxAndFee)
                {
                    PId = GetSubTotalId(),
                    Name = taxAndFee.Name,
                    Amount = Math.Round((Double)amount, 2),
                    VatAmount = Math.Round((Double)InvoiceSum.Vat * amount, 2),
                    QtyClientReceived = number
                });
            }
        }

        public void CalcExtraCharges(List<ExtraCharge> extraCharges)
        {
            if (extraCharges == null || extraCharges.Count == 0)
                return;

            var id = GetSubTotalId();

            foreach (var extraCharge in extraCharges.OrderBy(x => x.Id))
            {
                PreviewChargeItems.Add(new InvoiceItems(extraCharge)
                {
                    PId = ++id,
                    Name = extraCharge.Name,
                    Amount = Math.Round((Double)extraCharge.Amount, 2),
                    VatAmount = Math.Round((Double)InvoiceSum.Vat * extraCharge.Amount, 2),
                });
            }
        }

        public ObservableCollection<InvoiceItems> RefreshTaxAndCharges(List<TaxAndFees> taxes, ObservableCollection<InvoiceItems> items)
        {
            foreach (var taxAndFee in taxes.OrderBy(x => x.Priority))
            {
                var taxPreview = items.FirstOrDefault(x => x.OriginalObject == taxAndFee);

                if (taxPreview != null && !taxPreview.IsSelected) continue;

                var amount = new double();
                switch (taxAndFee.UnitId)
                {
                    case (int) FeeUnitEnum.AED:
                        amount = taxAndFee.Number;
                        break;
                    case (int) FeeUnitEnum.Percentage:
                    {
                        var totalAmount = new double();

                        foreach (var preview in items.Where(x => x.IsSelected && x.OriginalObject is Department))
                        {
                            totalAmount += preview.Amount - preview.DiscountAmount;
                        }

                        amount = totalAmount * (taxAndFee.Number / 100);
                        break;
                    }
                }
                taxPreview.Amount = Math.Round((Double)amount, 2);
                taxPreview.VatAmount = Math.Round((Double)amount * InvoiceSum.Vat, 2);
            }

            return items;
        }

        public int GetSubTotalId()
        {
            var id = 1;

            if (PreviewChargeItems != null && PreviewChargeItems.Count != 0)
                id = PreviewChargeItems.Max(x => x.PId) +1;

            return id;
        }

        public void Clear()
        {
        }

        public List<InvoiceItems> GetExpressNotes(List<NoteHeader> notes)
        {
            var invoiceNotes = CalculateNoteHeader(notes);

            return invoiceNotes;
        }
    }
}
