using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using MakeOrderStu3.Models.PreanalyticsClasses;
using MakeOrderStu3.Models.QuestionnaireClasses;
using MakeOrderStu3.Singletones;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MakeOrderStu3.Models
{
    public static class Make
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static PreanalyticsRequest PreanalyticsRequest()
        {
            Order order = GetOrder.Get();
            PreanalyticsRequest result = new PreanalyticsRequest()
            {
                ContractCode = order.ContractCode,
                IncludeTransportContainer = "false",
                AnalyticsRequests = new List<AnalyticsRequest>()
            };
            for (int i = 0; i < order.OrderPositions.Count; i++)
            {
                logger.Info($"PreanalyticsRequest: order position {i} - {order.OrderPositions[i].Id}");
                if (order.OrderPositions[i].NomenclaturePosition.SelectedSpecimens != null)
                {
                    for (int j = 0; j < order.OrderPositions[i].NomenclaturePosition.SelectedSpecimens.Count; j++)
                    {
                        AnalyticsRequest ar = new AnalyticsRequest()
                        {
                            Id = order.OrderPositions[i].NomenclaturePosition.Id,
                            Guid = order.OrderPositions[i].Id,
                            SpecimenCode = order.OrderPositions[i].NomenclaturePosition.SelectedSpecimens[j].SpecimenCode
                        };
                        if (!string.IsNullOrWhiteSpace(order.OrderPositions[i].NomenclaturePosition.SelectedSpecimens[j].BodysiteCode))
                        {
                            ar.BodyciteCode = order.OrderPositions[i].NomenclaturePosition.SelectedSpecimens[j]?.BodysiteCode;
                        }

                        if (!string.IsNullOrWhiteSpace(order.OrderPositions[i].NomenclaturePosition.SelectedSpecimens[j]?.ContainerType))
                        {
                            ar.ContainerType = order.OrderPositions[i].NomenclaturePosition.SelectedSpecimens[j]?.ContainerType;
                        }
                        result.AnalyticsRequests.Add(ar);
                    }
                }
                for (int j = 0; j < order.OrderPositions[i].NomenclaturePosition.RequiredSpecimen.Count; j++)
                {
                    AnalyticsRequest ar = new AnalyticsRequest()
                    {
                        Id = order.OrderPositions[i].NomenclaturePosition.Id,
                        Guid = order.OrderPositions[i].Id,
                        SpecimenCode = order.OrderPositions[i].NomenclaturePosition.RequiredSpecimen[j].SpecimenCode                        
                    };
                    if (!string.IsNullOrWhiteSpace(order.OrderPositions[i].NomenclaturePosition.RequiredSpecimen[j].BodysiteCode))
                    {
                        ar.BodyciteCode = order.OrderPositions[i].NomenclaturePosition.RequiredSpecimen[j]?.BodysiteCode;
                    }

                    if (!string.IsNullOrWhiteSpace(order.OrderPositions[i].NomenclaturePosition.RequiredSpecimen[j]?.ContainerType))
                    {
                        ar.ContainerType = order.OrderPositions[i].NomenclaturePosition.RequiredSpecimen[j]?.ContainerType;
                    }
                    result.AnalyticsRequests.Add(ar);
                }
            }
            return result;
        }

        public static string ResultBundle(string nomenclatureListInOrder, string preanalyticsResult, string questionnaireResult)
        {
            List<OrderPosition> orderPositions;
            AnalyticsResponse analyticsResponse;
            QuestionnaireResponse questionnaireResponse;
            SerializerSettings settings = new SerializerSettings()
            {
                Pretty = true
            };
            var serializer = new FhirJsonSerializer(settings);
            string patientUrl;
            try
            {
                orderPositions = JsonConvert.DeserializeObject<List<OrderPosition>>(nomenclatureListInOrder, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                analyticsResponse = JsonConvert.DeserializeObject<AnalyticsResponse>(preanalyticsResult, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                questionnaireResponse = string.IsNullOrWhiteSpace(questionnaireResult) ?
                    new QuestionnaireResponse()
                    {
                        Meta = new Meta()
                        {
                            Security = new List<Coding>()
                    {
                        new Coding("read", "service"),
                    }
                        },
                        Source = new ResourceReference($"patient/{GetOrder.Get().PatientUrl.Split('/').Last()}"),
                        Status = Hl7.Fhir.Model.QuestionnaireResponse.QuestionnaireResponseStatus.Completed
                    } : JsonConvert.DeserializeObject<QuestionnaireResponse>(preanalyticsResult, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
                patientUrl = GetOrder.Get().PatientUrl;
            }
            catch (Exception ex)
            {
                return $"Не удалось получить все объекты, нужные для создания Bundle, по причине:\r\n{ex}";
            }
            StringBuilder sb = new StringBuilder();
            orderPositions.Select(x => sb.Append($"{x}, ")).Count();
            logger.Info($"order positions\r\n{nomenclatureListInOrder}");
            logger.Info($"analyticsResponse\r\n{analyticsResponse}");
            logger.Info($"order positions - {sb.ToString().Trim().Trim(',')}");
            logger.Info("объекты, из которых собирается заказ, созданы");
            ResourceReference patientRelativeUrl;
            try
            {
                patientRelativeUrl = new ResourceReference($"patient/{GetOrder.Get().PatientUrl.Split('/').Last()}");
            }
            catch (Exception ex)
            {
                return $"Не удалось получить ссылку на пациента по причине:\r\n{ex}";
            }
            Bundle.EntryComponent orderProcessingTaskEntry = new Bundle.EntryComponent()
            {
                FullUrl = "urn:uuid:" + Guid.NewGuid().ToString(),
                Resource = new Task()
                {
                    Meta = new Meta()
                    {
                        Security = new List<Coding>()
                        {
                            new Coding("read", "service"),
                            new Coding("updatebody", "service")
                        }
                    },
                    Status = Task.TaskStatus.Requested,
                    Code = new CodeableConcept()
                    {
                        Coding = new List<Coding>()
                        {
                            new Coding(@"https://api.medlinx.online/terminology/task_type", "OrderProcessingTask")
                        }
                    }
                }
            };            
            logger.Info($"orderProcessingTaskEntry создан\r\n{serializer.SerializeToString(orderProcessingTaskEntry)}");
            List<Bundle.EntryComponent> entries = new List<Bundle.EntryComponent>();
            entries.Add(orderProcessingTaskEntry);
            Bundle.EntryComponent entry;
            Dictionary<string, List<string>> procedureRequestToSpecimenRelations = new Dictionary<string, List<string>>();
            for (int i = 0; i < analyticsResponse.Specimens.Length; i++)
            {
                entry = new Bundle.EntryComponent()
                {
                    FullUrl = "urn:uuid:" + Guid.NewGuid().ToString(),
                    Resource = new Specimen()
                    {
                        Meta = new Meta()
                        {
                            Security = new List<Coding>()
                            {
                                new Coding("read", "service"),
                            }
                        },
                        Type = new CodeableConcept()
                        {
                            Coding = new List<Coding>()
                            {
                                new Coding(@"https://api.medlinx.online/terminology/specimen-type", analyticsResponse.Specimens[i].Code.ToString())
                            }
                        },
                        Subject = patientRelativeUrl,
                        Collection = new Specimen.CollectionComponent()
                        {
                            Collected = new FhirDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:dd")),
                            Method = new CodeableConcept()
                            {
                                Coding = new List<Coding>()
                                {
                                    new Coding(@"https://api.medlinx.online/terminology/treatment_protocol", analyticsResponse.Specimens[i].Collection.Method)
                                }
                            }
                        },
                        Container = new List<Specimen.ContainerComponent>()
                        {
                            new Specimen.ContainerComponent()
                            {
                                Identifier = new List<Identifier>()
                                {
                                    new Identifier("http://helix.ru/codes/labels", "0000000000")
                                },
                                Type = new CodeableConcept()
                                {
                                    Coding = new List<Coding>()
                                    {
                                        new Coding(@"https://api.medlinx.online/terminology/specimen_type", analyticsResponse.Specimens[i].Container[0].Type)
                                    }
                                },
                                SpecimenQuantity = new SimpleQuantity()
                                {
                                    Value = analyticsResponse.Specimens[i].Collection.Quantity
                                }
                            }
                        },
                        Extension = new List<Extension>()
                        {
                            new Extension()
                            {
                                Url = @"https://api.medlinx.online/extra/supportingInfo",
                                Value = new FhirString(analyticsResponse.Specimens[i].SupportingInfo)
                            }
                        }
                    }
                };
                entries.Add(entry);
                procedureRequestToSpecimenRelations.Add(entry.FullUrl, analyticsResponse.Specimens[i].Guids);

                sb.Clear();
                analyticsResponse.Specimens[i].Guids.Select(x => sb.Append(x + ", ")).Count();
                logger.Info($" {entry.FullUrl} - {sb.ToString().Trim().Trim(',')}");
            }
            logger.Info("specimens созданы");
            string orderNumber = Guid.NewGuid().ToString();            
            for (int i = 0; i < orderPositions.Count; i++)
            {                
                var specimens = procedureRequestToSpecimenRelations.Where(x => x.Value.Contains(orderPositions[i].Id)).Select(x => new ResourceReference(x.Key)).ToList();
                logger.Info($"orderPositions Id - {orderPositions[i].Id}, specimens.Count = {specimens.Count}");
                entry = new Bundle.EntryComponent()
                {
                    FullUrl = "urn:uuid:" + Guid.NewGuid().ToString(),
                    Resource = new ProcedureRequest()
                    {
                        Meta = new Meta()
                        {
                            Security = new List<Coding>()
                                {
                                    new Coding("read", "service"),
                                }
                        },
                        Identifier = new List<Identifier>()
                            {
                                new Identifier(@"http://my.organization.org", Guid.NewGuid().ToString())
                            },
                        Requisition = new Identifier(@"http://my.organization.org/order-number", orderNumber),
                        SupportingInfo = new List<ResourceReference>()
                            {
                                new ResourceReference()
                                {
                                    Identifier = new Identifier(@"http://helix.ru/codes/contract", GetOrder.Get().ContractCode)
                                }
                            },
                        Status = RequestStatus.Active,
                        Code = new CodeableConcept()
                        {
                            Coding = new List<Coding>()
                                {
                                    new Coding(@"http://api.medlinx.online/terminology/nomenclature", orderPositions[i].NomenclaturePosition.Id)
                                },
                            Text = orderPositions[i].NomenclaturePosition.Caption
                        },
                        Subject = patientRelativeUrl,
                        Occurrence = new FhirDateTime(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:dd+03:00")),
                        Specimen = specimens,
                        Note = new List<Annotation>()
                            {
                                new Annotation()
                                {
                                    Text = string.Empty
                                }
                            }
                    }
                };
                entries.Add(entry);
                entries.Add(
                    new Bundle.EntryComponent()
                    {
                        FullUrl = "urn:uuid:" + Guid.NewGuid().ToString(),
                        Resource = new Task()
                        {
                            Meta = new Meta()
                            {
                                Security = new List<Coding>()
                                {
                                    new Coding("read", "service"),
                                    new Coding("updatebody", "service")
                                }
                            },
                            BasedOn = new List<ResourceReference>()
                            {
                                new ResourceReference(entry.FullUrl)
                            },
                            PartOf = new List<ResourceReference>()
                            {
                                new ResourceReference(orderProcessingTaskEntry.FullUrl)
                            },
                            Status = Task.TaskStatus.Requested
                        }
                    }
                );
            }
            logger.Info("procedureRequests и Tasks созданы");
            entries.Add(
                new Bundle.EntryComponent()
                {
                    FullUrl = "urn:uuid:" + Guid.NewGuid().ToString(),
                    Resource = new QuestionnaireResponse()
                    {
                        Item = GetOrder.Get().GroupResultItems,
                        Meta = new Meta()
                        {
                            Security = new List<Coding>()
                            {
                                new Coding("read", "service"),
                            }
                        },
                        Source = new ResourceReference($"patient/{GetOrder.Get().PatientUrl.Split('/').Last()}"),
                        Status = Hl7.Fhir.Model.QuestionnaireResponse.QuestionnaireResponseStatus.Completed,
                        Subject = new ResourceReference(entries.Where(x => x.Resource is ProcedureRequest).Select(y => y.FullUrl).FirstOrDefault())
                    }
                });

            Bundle resultBundle = new Bundle()
            {
                Type = Bundle.BundleType.Collection,
                Entry = entries
            };
            logger.Info("resultBundle создан");
            
            return serializer.SerializeToString(resultBundle);
        }

        public static string Answers(List<QuestionnaireAnswer> answers)
        {
            try
            {
                var linkIds = answers.Select(x => x.LinkId).Distinct().ToList();
                StringBuilder sb = new StringBuilder();
                linkIds.Select(x => sb.Append(x + ", ")).Count();
                logger.Info($"{linkIds.Count} {sb}");
                List<QuestionnaireAnswer> answerItems;
                List<QuestionnaireResponse.ItemComponent> groupResultItems = new List<QuestionnaireResponse.ItemComponent>();
                List<QuestionnaireResponse.ItemComponent> answerResultItems;
                foreach (var linkId in linkIds)
                {
                    QuestionnaireResponse.AnswerComponent answerComponent;
                    answerItems = new List<QuestionnaireAnswer>();
                    answerResultItems = new List<QuestionnaireResponse.ItemComponent>();
                    answerItems = answers.Where(x => x.LinkId == linkId).ToList();
                    foreach (var answerItem in answerItems)
                    {
                        answerComponent = null;
                        if (answerItem.Item.Type == "integer" && answerItem.ResultValue != null)
                        {
                            logger.Info($"integer {answerItem.Item.Text} - {answerItem.ResultValue}");
                            answerComponent = new QuestionnaireResponse.AnswerComponent()
                            {
                                Value = new Integer(int.Parse((string)answerItem.ResultValue))
                            };
                        }
                        else if (answerItem.Item.Type == "boolean" && answerItem.ResultValue != null)
                        {
                            logger.Info($"boolean {answerItem.Item.Text} - {answerItem.ResultValue}");
                            answerComponent = new QuestionnaireResponse.AnswerComponent()
                            {
                                Value = new FhirBoolean(((string)answerItem.ResultValue).ToUpper() == "Y")
                            };
                        }
                        else if (answerItem.Item.Type == "date" && answerItem.ResultValue != null)
                        {
                            logger.Info($"date {answerItem.Item.Text} - {answerItem.ResultValue}");
                            answerComponent = new QuestionnaireResponse.AnswerComponent()
                            {
                                Value = new Date(DateTime.Parse((string)answerItem.ResultValue).ToString("yyyy-MM-dd"))
                            };
                        }
                        else if (answerItem.Item.Type == "datetime" && answerItem.ResultValue != null)
                        {
                            logger.Info($"datetime {answerItem.Item.Text} - {answerItem.ResultValue}");
                            answerComponent = new QuestionnaireResponse.AnswerComponent()
                            {
                                Value = new Date(DateTime.Parse((string)answerItem.ResultValue).ToString("yyyy-MM-ddThh-mm-ss"))
                            };
                        }
                        else if (answerItem.Item.Type == "decimal" && answerItem.ResultValue != null)
                        {
                            logger.Info($"decimal {answerItem.Item.Text} - {answerItem.ResultValue}");
                            answerComponent = new QuestionnaireResponse.AnswerComponent()
                            {
                                Value = new FhirDecimal(decimal.Parse((string)answerItem.ResultValue))
                            };
                        }
                        else if (answerItem.Item.Type == "choice" && answerItem.ResultValue != null)
                        {
                            logger.Info($"choice {answerItem.Item.Text} - {answerItem.ResultValue}");
                            answerComponent = new QuestionnaireResponse.AnswerComponent()
                            {
                                Value = new Coding(((QuestionnaireOption)answerItem.ResultValue).ValueCoding.System, ((QuestionnaireOption)answerItem.ResultValue).ValueCoding.Code)
                            };
                        }
                        else
                        {
                            if (answerItem.ResultValue != null)
                            {
                                logger.Info($"string or smthng {answerItem.Item.Text} - {answerItem.ResultValue}");
                                answerComponent = new QuestionnaireResponse.AnswerComponent()
                                {
                                    Value = new FhirString((string)answerItem.ResultValue)
                                };
                            }                            
                        }
                        if (answerComponent != null)
                        {
                            answerResultItems.Add(new QuestionnaireResponse.ItemComponent()
                            {
                                Text = answerItem.Item.Text,
                                LinkId = answerItem.Item.LinkId,
                                Answer = new List<QuestionnaireResponse.AnswerComponent>()
                                {
                                    answerComponent
                                }
                            });
                        }                        
                    }
                    groupResultItems.Add
                    (
                        new QuestionnaireResponse.ItemComponent()
                        {
                            LinkId = linkId,
                            Text = answerItems.Select(x => x.Text).FirstOrDefault(),
                            Item = answerResultItems
                        }
                    );
                }
                
                GetOrder.AddResource(groupResultItems);
                SerializerSettings sett = new SerializerSettings()
                {
                    Pretty = true
                };
                var serializer = new FhirJsonSerializer(sett);
                var parser = new FhirJsonParser();
                StringBuilder result = new StringBuilder();
                groupResultItems.Select(x => result.AppendLine(serializer.SerializeToString(x) + ",")).Count();
                return result.ToString().Trim(',');
            }
            catch (Exception ex)
            {
                logger.Error($"Не удалось добавить ответы на опросник в заказ по причине:\r\n{ex}");
                throw new Exception($"Не удалось добавить ответы на опросник в заказ по причине:\r\n{ex}");
            }            
        }
    }
}